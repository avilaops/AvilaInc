import express, { Request, Response } from 'express';
import { azureDevOpsService } from '../services/azure.service.js';

const router = express.Router();

// GET /api/azure/projects - Lista todos os projetos
router.get('/projects', async (_req: Request, res: Response) => {
    try {
        const result = await azureDevOpsService.getProjects();
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/azure/builds - Lista builds (opcionalmente filtrar por projeto)
router.get('/builds', async (req: Request, res: Response) => {
    try {
        const { project } = req.query;
        const result = await azureDevOpsService.getBuilds(project as string);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/azure/releases - Lista releases (opcionalmente filtrar por projeto)
router.get('/releases', async (req: Request, res: Response) => {
    try {
        const { project } = req.query;
        const result = await azureDevOpsService.getReleases(project as string);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/azure/workitems - Lista work items (opcionalmente filtrar por projeto)
router.get('/workitems', async (req: Request, res: Response) => {
    try {
        const { project } = req.query;
        const result = await azureDevOpsService.getWorkItems(project as string);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// POST /api/azure/builds - Cria um novo build
router.post('/builds', async (req: Request, res: Response) => {
    try {
        const { projectName, buildDefinitionId, parameters } = req.body;
        const result = await azureDevOpsService.createBuild(projectName, buildDefinitionId, parameters);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

export default router;