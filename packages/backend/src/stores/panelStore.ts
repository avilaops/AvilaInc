import { promises as fs } from 'fs';
import path from 'path';
import {
  automationCatalog,
  CapabilityCreatePayload,
  CapabilityStatus,
  CapabilityUpdatePayload,
  createInitialPanelState,
  deriveProgressFromStatus,
  PanelCapability,
  PanelCategory,
  PanelState,
} from '@vizzio/core';

interface CapabilityFilter {
  status?: CapabilityStatus;
  tag?: string;
  search?: string;
}

function clampProgress(value?: number): number {
  if (typeof value !== 'number' || Number.isNaN(value)) {
    return 0;
  }
  return Math.min(100, Math.max(0, value));
}

function cloneState<T>(payload: T): T {
  return JSON.parse(JSON.stringify(payload));
}

export class PanelStore {
  private readonly filePath: string;
  private state: PanelState | null = null;
  private queue: Promise<void> = Promise.resolve();

  constructor(filePath?: string) {
    this.filePath =
      filePath ?? path.resolve(__dirname, '../data/panel-state.json');
  }

  private async withLock<T>(handler: () => Promise<T>): Promise<T> {
    let resolveQueue: () => void = () => undefined;
    const waitPrevious = this.queue;
    this.queue = new Promise<void>((resolve) => {
      resolveQueue = resolve;
    });

    await waitPrevious.catch(() => undefined);

    try {
      const result = await handler();
      resolveQueue();
      return result;
    } catch (error) {
      resolveQueue();
      throw error;
    }
  }

  private async ensureReady(): Promise<void> {
    if (this.state) {
      return;
    }
    try {
      const raw = await fs.readFile(this.filePath, 'utf-8');
      this.state = JSON.parse(raw) as PanelState;
    } catch (error: any) {
      if (error?.code !== 'ENOENT') {
        throw error;
      }
      await this.seed();
    }
  }

  private async seed(): Promise<void> {
    const timestamp = new Date().toISOString();
    this.state = createInitialPanelState(automationCatalog, timestamp);
    await this.persist();
  }

  private async persist(): Promise<void> {
    if (!this.state) {
      throw new Error('Panel state not initialized');
    }
    await fs.mkdir(path.dirname(this.filePath), { recursive: true });
    await fs.writeFile(
      this.filePath,
      JSON.stringify(this.state, null, 2),
      'utf-8'
    );
  }

  private locateCapability(id: string): {
    capability: PanelCapability;
    category: PanelCategory;
  } | null {
    if (!this.state) {
      return null;
    }

    for (const category of this.state.categories) {
      const capability = category.capabilities.find((item) => item.id === id);
      if (capability) {
        return { capability, category };
      }
    }

    return null;
  }

  async getState(): Promise<PanelState> {
    await this.ensureReady();
    return cloneState(this.state!);
  }

  async getCapability(id: string): Promise<PanelCapability | null> {
    await this.ensureReady();
    const located = this.locateCapability(id);
    return located ? cloneState(located.capability) : null;
  }

  async listCapabilities(filter: CapabilityFilter = {}): Promise<PanelCapability[]> {
    await this.ensureReady();
    const { status, tag, search } = filter;
    const searchLower = search?.toLowerCase();

    const capabilities = this.state!.categories.flatMap((category) =>
      category.capabilities.filter((capability) => {
        const matchStatus = status ? capability.status === status : true;
        const matchTag = tag
          ? capability.tags?.some(
              (current) => current.toLowerCase() === tag.toLowerCase()
            )
          : true;
        const matchSearch = searchLower
          ? capability.name.toLowerCase().includes(searchLower) ||
            capability.description.toLowerCase().includes(searchLower)
          : true;
        return matchStatus && matchTag && matchSearch;
      })
    );

    return cloneState(capabilities);
  }

  async updateCapability(
    id: string,
    payload: CapabilityUpdatePayload
  ): Promise<PanelCapability> {
    return this.withLock(async () => {
      await this.ensureReady();
      const located = this.locateCapability(id);
      if (!located) {
        throw new Error(`Capability '${id}' not found`);
      }

      const { capability, category } = located;
      const now = new Date().toISOString();
      const previousStatus = capability.status;

      if (payload.status && payload.status !== capability.status) {
        capability.status = payload.status;
        capability.statusHistory.push({
          status: payload.status,
          changedAt: now,
          note: payload.note,
        });
        if (payload.progress === undefined) {
          capability.progress = deriveProgressFromStatus(payload.status);
        }
      }

      if (payload.name !== undefined) {
        capability.name = payload.name;
      }

      if (payload.description !== undefined) {
        capability.description = payload.description;
      }

      if (payload.owner !== undefined) {
        capability.owner = payload.owner || undefined;
      }

      if (payload.package !== undefined) {
        capability.package = payload.package || undefined;
      }

      if (payload.docs !== undefined) {
        capability.docs = payload.docs || undefined;
      }

      if (payload.tags) {
        capability.tags = payload.tags;
      }

      if (payload.progress !== undefined) {
        capability.progress = clampProgress(payload.progress);
      }

      capability.updatedAt = now;
      category.capabilities = category.capabilities.map((item) =>
        item.id === capability.id ? capability : item
      );

      this.state!.updatedAt = now;
      await this.persist();

      return cloneState(capability);
    });
  }

  async createCapability(
    payload: CapabilityCreatePayload
  ): Promise<PanelCapability> {
    return this.withLock(async () => {
      await this.ensureReady();
      const category = this.state!.categories.find(
        (item) => item.id === payload.categoryId
      );
      if (!category) {
        throw new Error(`Category '${payload.categoryId}' not found`);
      }

      if (category.capabilities.some((item) => item.id === payload.id)) {
        throw new Error(`Capability '${payload.id}' already exists`);
      }

      const now = new Date().toISOString();
      const status = payload.status ?? 'planned';
      const capability: PanelCapability = {
        id: payload.id,
        name: payload.name,
        description: payload.description,
        status,
        package: payload.package,
        docs: payload.docs,
        tags: payload.tags ?? [],
        owner: payload.owner,
        progress: clampProgress(
          payload.progress ?? deriveProgressFromStatus(status)
        ),
        categoryId: category.id,
        categoryLabel: category.label,
        icon: category.icon,
        createdAt: now,
        updatedAt: now,
        statusHistory: [
          {
            status,
            changedAt: now,
            note: payload.note,
          },
        ],
      };

      category.capabilities.push(capability);
      this.state!.updatedAt = now;
      await this.persist();
      return cloneState(capability);
    });
  }
}
