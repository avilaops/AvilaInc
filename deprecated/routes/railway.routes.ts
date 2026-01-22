import express, { Request, Response } from 'express';
import { railwayService } from '../services/railway.service.js';

const router = express.Router();

// GET /api/railway/projects - Lista todos os projetos
router.get('/projects', async (_req: Request, res: Response) => {
    try {
        const result = await railwayService.getProjects();
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/railway/projects/:id - Detalhes de um projeto específico
router.get('/projects/:id', async (req: Request, res: Response) => {
    try {
        const id = Array.isArray(req.params.id) ? req.params.id[0] : req.params.id;
        const result = await railwayService.getProjectDetails(id);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// POST /api/railway/projects/:id/deploy - Faz deploy de um projeto
router.post('/projects/:id/deploy', async (req: Request, res: Response) => {
    try {
        const id = Array.isArray(req.params.id) ? req.params.id[0] : req.params.id;
        const { environmentId } = req.body;
        const result = await railwayService.deployProject(id, environmentId);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

export default router;