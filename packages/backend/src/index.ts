import express from 'express';
import {
	CapabilityCreatePayload,
	CapabilityStatus,
	CapabilityUpdatePayload,
	devtoolsBlueprint,
	devtoolsChecklist,
} from '@vizzio/core';
import { PanelStore } from './stores/panelStore';

const app = express();
const PORT = Number(process.env.PORT ?? 4000);
const panelStore = new PanelStore();

app.use(express.json());

const asyncHandler = <T extends express.RequestHandler>(handler: T): T => {
	return (async (
		req: express.Request,
		res: express.Response,
		next: express.NextFunction
	) => {
		try {
			await Promise.resolve(handler(req, res, next));
		} catch (error) {
			next(error);
		}
	}) as unknown as T;
};

app.get('/health', (_req, res) => {
	res.json({ status: 'ok', timestamp: new Date().toISOString() });
});

app.get(
	'/panel',
	asyncHandler(async (_req, res) => {
		const state = await panelStore.getState();
		res.json({
			state,
			devtoolsBlueprint,
			devtoolsChecklist,
		});
	})
);

app.get(
	'/panel/capabilities',
	asyncHandler(async (req, res) => {
		const { tag, status, search } = req.query;
		const capabilities = await panelStore.listCapabilities({
			tag: tag ? String(tag) : undefined,
			status: status ? (String(status) as CapabilityStatus) : undefined,
			search: search ? String(search) : undefined,
		});
		res.json({ count: capabilities.length, items: capabilities });
	})
);

app.get(
	'/panel/capabilities/:id',
	asyncHandler(async (req, res) => {
		const capability = await panelStore.getCapability(req.params.id);
		if (!capability) {
			return res.status(404).json({ message: 'Capability not found' });
		}
		return res.json(capability);
	})
);

app.post(
	'/panel/capabilities',
	asyncHandler(async (req, res) => {
		const body = req.body as CapabilityCreatePayload;
		if (!body || !body.categoryId || !body.id || !body.name || !body.description) {
			return res.status(400).json({
				message:
					"Payload must include 'categoryId', 'id', 'name' and 'description' fields.",
			});
		}
		const capability = await panelStore.createCapability(body);
		res.status(201).json(capability);
	})
);

app.put(
	'/panel/capabilities/:id',
	asyncHandler(async (req, res) => {
		const payload = req.body as CapabilityUpdatePayload;
		const capability = await panelStore.updateCapability(req.params.id, payload);
		res.json(capability);
	})
);

app.get(
	'/panel/blueprint/:sectionId',
	(req, res) => {
		const section = devtoolsBlueprint.find((item) => item.id === req.params.sectionId);
		if (!section) {
			return res.status(404).json({ message: 'Blueprint section not found' });
		}
		res.json(section);
	}
);

app.get(
	'/panel/summary',
	asyncHandler(async (_req, res) => {
		const state = await panelStore.getState();
		const capabilitiesCount = state.categories.reduce(
			(total, category) => total + category.capabilities.length,
			0
		);
		res.json({
			capabilities: capabilitiesCount,
			categories: state.categories.length,
			blueprintSections: devtoolsBlueprint.length,
			checklistItems: devtoolsChecklist.length,
			updatedAt: state.updatedAt,
		});
	})
);

app.use(
	(err: any, _req: express.Request, res: express.Response, _next: express.NextFunction) => {
		const message = err?.message ?? 'Unexpected error';
		const statusCode = /not found/i.test(message) ? 404 : 400;
		if (statusCode >= 500 || process.env.NODE_ENV !== 'production') {
			console.error('[panel-api] error:', err);
		}
		res.status(statusCode).json({ message });
	}
);

if (require.main === module) {
	app.listen(PORT, () => {
		console.log(`🚀 Backend panel API listening on http://localhost:${PORT}`);
	});
}

export { app };
export default app;
