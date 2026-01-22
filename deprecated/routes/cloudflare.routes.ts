import express, { Request, Response } from 'express';
import { cloudflareService } from '../services/cloudflare.service.js';

const router = express.Router();

// GET /api/cloudflare/zones - Lista zonas
router.get('/zones', async (_req: Request, res: Response) => {
    try {
        const result = await cloudflareService.getZones();
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/cloudflare/dns - Lista registros DNS (opcionalmente filtrar por zona)
router.get('/dns', async (req: Request, res: Response) => {
    try {
        const { zoneId } = req.query;
        const result = await cloudflareService.getDNSRecords(zoneId as string);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// POST /api/cloudflare/dns - Cria um novo registro DNS
router.post('/dns', async (req: Request, res: Response) => {
    try {
        const { zoneId, ...record } = req.body;
        const result = await cloudflareService.createDNSRecord(record, zoneId);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// PUT /api/cloudflare/dns/:recordId - Atualiza um registro DNS
router.put('/dns/:recordId', async (req: Request, res: Response) => {
    try {
        const recordId = Array.isArray(req.params.recordId) ? req.params.recordId[0] : req.params.recordId;
        const { zoneId, ...record } = req.body;
        const result = await cloudflareService.updateDNSRecord(recordId, record, zoneId);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// DELETE /api/cloudflare/dns/:recordId - Deleta um registro DNS
router.delete('/dns/:recordId', async (req: Request, res: Response) => {
    try {
        const recordId = Array.isArray(req.params.recordId) ? req.params.recordId[0] : req.params.recordId;
        const { zoneId } = req.body;
        const result = await cloudflareService.deleteDNSRecord(recordId, zoneId);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/cloudflare/analytics - Analytics da zona
router.get('/analytics', async (req: Request, res: Response) => {
    try {
        const { zoneId, since, until } = req.query;
        const result = await cloudflareService.getAnalytics(
            zoneId as string,
            since ? parseInt(since as string) : undefined,
            until ? parseInt(until as string) : undefined
        );
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// POST /api/cloudflare/cache/purge - Limpa cache
router.post('/cache/purge', async (req: Request, res: Response) => {
    try {
        const { zoneId, files } = req.body;
        const result = await cloudflareService.purgeCache(zoneId, files);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/cloudflare/pagerules - Lista page rules
router.get('/pagerules', async (req: Request, res: Response) => {
    try {
        const { zoneId } = req.query;
        const result = await cloudflareService.getPageRules(zoneId as string);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

export default router;