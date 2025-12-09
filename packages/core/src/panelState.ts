import {
  AutomationCapability,
  AutomationCategory,
  automationCatalog,
  CapabilityStatus,
} from './automationRegistry';

export interface CapabilityStatusEntry {
  status: CapabilityStatus;
  changedAt: string;
  note?: string;
}

export interface PanelCapability extends AutomationCapability {
  categoryId: string;
  categoryLabel: string;
  icon?: string;
  owner?: string;
  progress: number;
  createdAt: string;
  updatedAt: string;
  statusHistory: CapabilityStatusEntry[];
}

export interface PanelCategory
  extends Omit<AutomationCategory, 'capabilities'> {
  capabilities: PanelCapability[];
}

export interface PanelState {
  version: number;
  updatedAt: string;
  categories: PanelCategory[];
}

export interface CapabilityUpdatePayload {
  status?: CapabilityStatus;
  description?: string;
  owner?: string;
  progress?: number;
  tags?: string[];
  docs?: string;
  package?: string;
  name?: string;
  note?: string;
}

export interface CapabilityCreatePayload {
  categoryId: string;
  id: string;
  name: string;
  description: string;
  status?: CapabilityStatus;
  package?: string;
  docs?: string;
  tags?: string[];
  owner?: string;
  progress?: number;
  note?: string;
}

export const PANEL_STATE_VERSION = 1;

export function deriveProgressFromStatus(status: CapabilityStatus): number {
  switch (status) {
    case 'planned':
      return 5;
    case 'in-progress':
      return 45;
    case 'available':
      return 100;
    case 'blocked':
      return 25;
    case 'deprecated':
      return 0;
    default:
      return 0;
  }
}

export function normalizeCapability(
  category: AutomationCategory,
  capability: AutomationCapability,
  timestamp: string
): PanelCapability {
  return {
    ...capability,
    owner: capability.owner,
    progress:
      capability.progress ?? deriveProgressFromStatus(capability.status),
    categoryId: category.id,
    categoryLabel: category.label,
    icon: category.icon,
    createdAt: timestamp,
    updatedAt: timestamp,
    statusHistory: [
      {
        status: capability.status,
        changedAt: timestamp,
      },
    ],
  };
}

export function createInitialPanelState(
  catalog: AutomationCategory[] = automationCatalog,
  timestamp: string = new Date().toISOString()
): PanelState {
  return {
    version: PANEL_STATE_VERSION,
    updatedAt: timestamp,
    categories: catalog.map((category) => ({
      id: category.id,
      label: category.label,
      icon: category.icon,
      capabilities: category.capabilities.map((capability) =>
        normalizeCapability(category, capability, timestamp)
      ),
    })),
  };
}
