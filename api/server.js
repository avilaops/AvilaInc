/**
 * Ávila Inc - Gravatar API Server v0.1.0
 * Servidor Express para integração com Gravatar
 * 
 * @version 0.1.0
 * @date 2025-11-16
 */

require('dotenv').config();
const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const rateLimit = require('express-rate-limit');
const gravatarRoutes = require('./routes/gravatar-routes');

const app = express();
const PORT = process.env.PORT || 3000;
const API_VERSION = '0.1.0';

// Middleware de segurança
app.use(helmet());

// CORS configurado para produtos Ávila
app.use(cors({
    origin: [
        'https://salmon-island-0f049391e.3.azurestaticapps.net',
        'https://api.avila.inc',
        'https://*.avila.inc',
        'http://localhost:3000',
        'http://localhost:3001'
    ],
    credentials: true
}));

// Body parser
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Rate limiting
const limiter = rateLimit({
    windowMs: 1 * 60 * 1000, // 1 minuto
    max: 1000, // limite de requisições
    message: {
        success: false,
        error: 'Muitas requisições. Tente novamente em alguns instantes.'
    },
    standardHeaders: true,
    legacyHeaders: false
});

const batchLimiter = rateLimit({
    windowMs: 1 * 60 * 1000,
    max: 100,
    message: {
        success: false,
        error: 'Limite de batch requests excedido.'
    }
});

// Aplicar rate limiting
app.use('/api/v0/gravatar', limiter);
app.use('/api/v0/gravatar/avatars/batch', batchLimiter);

// Middleware de logging
app.use((req, res, next) => {
    const timestamp = new Date().toISOString();
    console.log(`[${timestamp}] ${req.method} ${req.path} - ${req.headers['x-product-name'] || 'unknown'}`);
    next();
});

// Health check
app.get('/health', (req, res) => {
    res.json({
        status: 'healthy',
        version: API_VERSION,
        timestamp: new Date().toISOString(),
        service: 'gravatar-api'
    });
});

// API Info
app.get('/api/v0', (req, res) => {
    res.json({
        name: 'Ávila Gravatar API',
        version: API_VERSION,
        description: 'API de integração com Gravatar para avatares de usuários',
        documentation: 'https://docs.avila.inc/gravatar',
        endpoints: {
            avatar: '/api/v0/gravatar/avatar/:email',
            batch: '/api/v0/gravatar/avatars/batch',
            product: '/api/v0/gravatar/product/:product/:email',
            profile: '/api/v0/gravatar/profile/:email',
            check: '/api/v0/gravatar/check/:email',
            hash: '/api/v0/gravatar/hash/:email'
        },
        themes: ['light', 'dark'],
        products: ['secreta', 'dashboard', 'pulse', 'archivus', 'barbara', 'shancrys']
    });
});

// Rotas Gravatar
app.use('/api/v0/gravatar', gravatarRoutes);

// 404 Handler
app.use((req, res) => {
    res.status(404).json({
        success: false,
        error: 'Endpoint não encontrado',
        path: req.path,
        version: API_VERSION
    });
});

// Error Handler
app.use((err, req, res, next) => {
    console.error('Error:', err);
    res.status(err.status || 500).json({
        success: false,
        error: err.message || 'Erro interno do servidor',
        version: API_VERSION
    });
});

// Start server
app.listen(PORT, () => {
    console.log(`
╔═══════════════════════════════════════════════════════════╗
║                                                           ║
║   🚀 Ávila Gravatar API v${API_VERSION}                        ║
║                                                           ║
║   Server running on: http://localhost:${PORT}              ║
║   Environment: ${process.env.NODE_ENV || 'development'}                       ║
║   Themes: light, dark                                     ║
║                                                           ║
║   Endpoints:                                              ║
║   - GET  /health                                          ║
║   - GET  /api/v0                                          ║
║   - GET  /api/v0/gravatar/avatar/:email                   ║
║   - POST /api/v0/gravatar/avatars/batch                   ║
║   - GET  /api/v0/gravatar/product/:product/:email         ║
║                                                           ║
║   📚 Documentation: GRAVATAR_API_v0.md                    ║
║                                                           ║
╚═══════════════════════════════════════════════════════════╝
    `);
});

module.exports = app;
