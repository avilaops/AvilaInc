import express, { Request, Response } from 'express';
import { sentryService } from '../services/sentry.service.js';

const router = express.Router();

// GET /api/sentry/issues - Lista issues (opcionalmente filtrar por projeto)
router.get('/issues', async (req: Request, res: Response) => {
    try {
        const { project } = req.query;
        const result = await sentryService.getIssues(project as string);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/sentry/projects - Lista projetos
router.get('/projects', async (_req: Request, res: Response) => {
    try {
        const result = await sentryService.getProjects();
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/sentry/projects/:project/issues/:issueId/events - Lista eventos de uma issue
router.get('/projects/:project/issues/:issueId/events', async (req: Request, res: Response) => {
    try {
        const project = Array.isArray(req.params.project) ? req.params.project[0] : req.params.project;
        const issueId = Array.isArray(req.params.issueId) ? req.params.issueId[0] : req.params.issueId;
        const result = await sentryService.getEvents(project, issueId);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// PUT /api/sentry/projects/:project/issues/:issueId/resolve - Resolve uma issue
router.put('/projects/:project/issues/:issueId/resolve', async (req: Request, res: Response) => {
    try {
        const project = Array.isArray(req.params.project) ? req.params.project[0] : req.params.project;
        const issueId = Array.isArray(req.params.issueId) ? req.params.issueId[0] : req.params.issueId;
        const result = await sentryService.resolveIssue(project, issueId);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/sentry/stats - Estatísticas (opcionalmente filtrar por projeto)
router.get('/stats', async (req: Request, res: Response) => {
    try {
        const { project } = req.query;
        const result = await sentryService.getStats(project as string);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

export default router;