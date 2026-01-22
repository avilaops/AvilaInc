import express, { Request, Response } from 'express';
import { linkedinService } from '../services/linkedin.service.js';

const router = express.Router();

// GET /api/linkedin/profile - Perfil do usuário
router.get('/profile', async (_req: Request, res: Response) => {
    try {
        const result = await linkedinService.getProfile();
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/linkedin/connections - Conexões do usuário
router.get('/connections', async (_req: Request, res: Response) => {
    try {
        const result = await linkedinService.getConnections();
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/linkedin/updates - Atualizações da rede
router.get('/updates', async (_req: Request, res: Response) => {
    try {
        const result = await linkedinService.getNetworkUpdates();
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// POST /api/linkedin/posts - Cria uma nova postagem
router.post('/posts', async (req: Request, res: Response) => {
    try {
        const { text } = req.body;
        const result = await linkedinService.postUpdate(text);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/linkedin/search - Busca pessoas
router.get('/search', async (req: Request, res: Response) => {
    try {
        const { keywords, count } = req.query;
        const result = await linkedinService.searchPeople(
            keywords as string,
            count ? parseInt(count as string) : 10
        );
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/linkedin/companies/:companyId/updates - Atualizações de uma empresa
router.get('/companies/:companyId/updates', async (req: Request, res: Response) => {
    try {
        const companyId = Array.isArray(req.params.companyId) ? req.params.companyId[0] : req.params.companyId;
        const result = await linkedinService.getCompanyUpdates(companyId);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// POST /api/linkedin/messages - Envia uma mensagem
router.post('/messages', async (req: Request, res: Response) => {
    try {
        const { recipientId, message } = req.body;
        const result = await linkedinService.sendMessage(recipientId, message);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

export default router;