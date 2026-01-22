import express from 'express';
import OpenAI from 'openai';
import { createRequire } from 'module';
const require = createRequire(import.meta.url);
import config from '../config/config.js';

const router = express.Router();

// Inicializar OpenAI client
const openai = new OpenAI({
    apiKey: config.openai.key
});

// Middleware para verificar se OpenAI está configurado
const checkOpenAI = (req, res, next) => {
    if (!config.openai.key) {
        return res.status(503).json({
            error: 'OpenAI não configurado',
            message: 'Configure OPENAI_API_KEY no arquivo .env'
        });
    }
    next();
};

// GET /openai/usage - Obter uso da API
router.get('/usage', checkOpenAI, async (req, res) => {
    try {
        const usage = await openai.usage.list({
            limit: 30,
            start_date: Math.floor(Date.now() / 1000) - (30 * 24 * 60 * 60), // Últimos 30 dias
            end_date: Math.floor(Date.now() / 1000)
        });

        res.json({
            success: true,
            data: usage.data,
            total: usage.data.length
        });
    } catch (error) {
        console.error('Erro ao obter uso da OpenAI:', error);
        res.status(500).json({
            error: 'Erro interno',
            message: error.message
        });
    }
});

// GET /openai/models - Listar modelos disponíveis
router.get('/models', checkOpenAI, async (req, res) => {
    try {
        const models = await openai.models.list();

        // Filtrar apenas modelos GPT e ordenar por data de criação
        const gptModels = models.data
            .filter(model => model.id.includes('gpt'))
            .sort((a, b) => new Date(b.created) - new Date(a.created));

        res.json({
            success: true,
            models: gptModels,
            total: gptModels.length
        });
    } catch (error) {
        console.error('Erro ao listar modelos:', error);
        res.status(500).json({
            error: 'Erro interno',
            message: error.message
        });
    }
});

// POST /openai/chat - Chat completion
router.post('/chat', checkOpenAI, async (req, res) => {
    try {
        const { messages, model = 'gpt-3.5-turbo', temperature = 0.7, max_tokens = 1000 } = req.body;

        if (!messages || !Array.isArray(messages)) {
            return res.status(400).json({
                error: 'Mensagens inválidas',
                message: 'Campo "messages" deve ser um array'
            });
        }

        const completion = await openai.chat.completions.create({
            model,
            messages,
            temperature,
            max_tokens
        });

        res.json({
            success: true,
            response: completion.choices[0].message,
            usage: completion.usage,
            model: completion.model
        });
    } catch (error) {
        console.error('Erro no chat completion:', error);
        res.status(500).json({
            error: 'Erro interno',
            message: error.message
        });
    }
});

// POST /openai/completions - Text completion (legacy)
router.post('/completions', checkOpenAI, async (req, res) => {
    try {
        const { prompt, model = 'text-davinci-003', temperature = 0.7, max_tokens = 1000 } = req.body;

        if (!prompt) {
            return res.status(400).json({
                error: 'Prompt inválido',
                message: 'Campo "prompt" é obrigatório'
            });
        }

        const completion = await openai.completions.create({
            model,
            prompt,
            temperature,
            max_tokens
        });

        res.json({
            success: true,
            response: completion.choices[0].text,
            usage: completion.usage,
            model: completion.model
        });
    } catch (error) {
        console.error('Erro na completion:', error);
        res.status(500).json({
            error: 'Erro interno',
            message: error.message
        });
    }
});

// POST /openai/images/generate - Gerar imagens
router.post('/images/generate', checkOpenAI, async (req, res) => {
    try {
        const { prompt, n = 1, size = '1024x1024' } = req.body;

        if (!prompt) {
            return res.status(400).json({
                error: 'Prompt inválido',
                message: 'Campo "prompt" é obrigatório'
            });
        }

        const image = await openai.images.generate({
            prompt,
            n,
            size
        });

        res.json({
            success: true,
            images: image.data
        });
    } catch (error) {
        console.error('Erro ao gerar imagem:', error);
        res.status(500).json({
            error: 'Erro interno',
            message: error.message
        });
    }
});

// POST /openai/embeddings - Criar embeddings
router.post('/embeddings', checkOpenAI, async (req, res) => {
    try {
        const { input, model = 'text-embedding-ada-002' } = req.body;

        if (!input) {
            return res.status(400).json({
                error: 'Input inválido',
                message: 'Campo "input" é obrigatório'
            });
        }

        const embedding = await openai.embeddings.create({
            model,
            input
        });

        res.json({
            success: true,
            embeddings: embedding.data,
            usage: embedding.usage
        });
    } catch (error) {
        console.error('Erro ao criar embedding:', error);
        res.status(500).json({
            error: 'Erro interno',
            message: error.message
        });
    }
});

// POST /openai/moderation - Moderar conteúdo
router.post('/moderation', checkOpenAI, async (req, res) => {
    try {
        const { input } = req.body;

        if (!input) {
            return res.status(400).json({
                error: 'Input inválido',
                message: 'Campo "input" é obrigatório'
            });
        }

        const moderation = await openai.moderations.create({
            input
        });

        res.json({
            success: true,
            results: moderation.results
        });
    } catch (error) {
        console.error('Erro na moderação:', error);
        res.status(500).json({
            error: 'Erro interno',
            message: error.message
        });
    }
});

// GET /openai/billing - Informações de cobrança (se disponível)
router.get('/billing', checkOpenAI, async (req, res) => {
    try {
        // Nota: A API de cobrança pode não estar disponível para todas as contas
        const billing = await openai.billing.usage({
            start_date: Math.floor(Date.now() / 1000) - (30 * 24 * 60 * 60),
            end_date: Math.floor(Date.now() / 1000)
        });

        res.json({
            success: true,
            billing: billing
        });
    } catch (error) {
        console.error('Erro ao obter cobrança:', error);
        res.status(500).json({
            error: 'Erro interno',
            message: error.message
        });
    }
});

// GET /openai/status - Status da integração
router.get('/status', (req, res) => {
    const isConfigured = !!(config.openai.key && config.openai.key.startsWith('sk-'));

    res.json({
        success: true,
        configured: isConfigured,
        version: '4.52.7',
        endpoints: [
            'GET /openai/usage',
            'GET /openai/models',
            'POST /openai/chat',
            'POST /openai/completions',
            'POST /openai/images/generate',
            'POST /openai/embeddings',
            'POST /openai/moderation',
            'GET /openai/billing',
            'GET /openai/status'
        ]
    });
});

export default router;