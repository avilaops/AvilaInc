import express, { Request, Response } from 'express';
import { dnsService } from '../services/dns.service.js';

const router = express.Router();

// GET /api/dns/domains - Lista todos os domínios
router.get('/domains', async (_req: Request, res: Response) => {
    try {
        const result = await dnsService.getDomains();
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/dns/domains/:domain/records - Lista registros DNS de um domínio
router.get('/domains/:domain/records', async (req: Request, res: Response) => {
    try {
        const domain = Array.isArray(req.params.domain) ? req.params.domain[0] : req.params.domain;
        const result = await dnsService.getDomainRecords(domain);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// POST /api/dns/domains/:domain/records - Cria um novo registro DNS
router.post('/domains/:domain/records', async (req: Request, res: Response) => {
    try {
        const domain = Array.isArray(req.params.domain) ? req.params.domain[0] : req.params.domain;
        const record = req.body;
        const result = await dnsService.createRecord(domain, record);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// PUT /api/dns/domains/:domain/records/:recordId - Atualiza um registro DNS
router.put('/domains/:domain/records/:recordId', async (req: Request, res: Response) => {
    try {
        const domain = Array.isArray(req.params.domain) ? req.params.domain[0] : req.params.domain;
        const recordId = Array.isArray(req.params.recordId) ? req.params.recordId[0] : req.params.recordId;
        const record = req.body;
        const result = await dnsService.updateRecord(domain, recordId, record);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// DELETE /api/dns/domains/:domain/records/:recordId - Deleta um registro DNS
router.delete('/domains/:domain/records/:recordId', async (req: Request, res: Response) => {
    try {
        const domain = Array.isArray(req.params.domain) ? req.params.domain[0] : req.params.domain;
        const recordId = Array.isArray(req.params.recordId) ? req.params.recordId[0] : req.params.recordId;
        const result = await dnsService.deleteRecord(domain, recordId);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/dns/pricing - Preços de domínio
router.get('/pricing', async (_req: Request, res: Response) => {
    try {
        const result = await dnsService.getDomainPricing();
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

export default router;