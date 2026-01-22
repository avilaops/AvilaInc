import express, { Request, Response } from 'express';
import { paypalService } from '../services/paypal.service.js';

const router = express.Router();

// GET /api/paypal/balance - Saldo da conta PayPal
router.get('/balance', async (_req: Request, res: Response) => {
    try {
        const result = await paypalService.getBalance();
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// GET /api/paypal/transactions - Lista transações
router.get('/transactions', async (req: Request, res: Response) => {
    try {
        const { startDate, endDate } = req.query;
        const result = await paypalService.getTransactions(
            startDate as string,
            endDate as string
        );
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// POST /api/paypal/payments - Cria um novo pagamento
router.post('/payments', async (req: Request, res: Response) => {
    try {
        const { amount, currency, description } = req.body;
        const result = await paypalService.createPayment(amount, currency, description);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// POST /api/paypal/payments/:paymentId/execute - Executa um pagamento
router.post('/payments/:paymentId/execute', async (req: Request, res: Response) => {
    try {
        const paymentId = Array.isArray(req.params.paymentId) ? req.params.paymentId[0] : req.params.paymentId;
        const { payerId } = req.body;
        const result = await paypalService.executePayment(paymentId, payerId);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// POST /api/paypal/refunds - Reembolsa um pagamento
router.post('/refunds', async (req: Request, res: Response) => {
    try {
        const { saleId, amount } = req.body;
        const result = await paypalService.refundPayment(saleId, amount);
        res.json(result);
    } catch (error: any) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

export default router;