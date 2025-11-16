/**
 * Barbara API - Gravatar Integration Routes v0.1.0
 * Endpoints REST para integração com Gravatar
 * Base URL: https://api.avila.inc
 * 
 * @version 0.1.0
 * @date 2025-11-16
 */

const express = require('express');
const router = express.Router();
const gravatar = require('../gravatar');

/**
 * GET /api/v0/gravatar/avatar/:email
 * Obtém URL do avatar do usuário com suporte a tema
 *
 * @param {string} email - Email do usuário (URL encoded)
 * @query {number} size - Tamanho do avatar (padrão: 200)
 * @query {string} default - Avatar padrão (mp, identicon, monsterid, wavatar, retro, robohash)
 * @query {string} theme - Tema (light, dark) - padrão: light
 * @returns {object} { email, avatarUrl, theme, themeConfig, hasCustomAvatar }
 */
router.get('/avatar/:email', async (req, res) => {
    try {
        const email = decodeURIComponent(req.params.email);
        const size = parseInt(req.query.size) || 200;
        const defaultAvatar = req.query.default || 'mp';
        const theme = req.query.theme || 'light';

        const avatarData = gravatar.getAvatarUrl(email, {
            size,
            default: defaultAvatar,
            theme
        });

        const hasCustomAvatar = await gravatar.hasGravatar(email);

        // Log analytics
        await gravatar.logAvatarUsage({
            endpoint: 'avatar',
            email,
            product: req.headers['x-product-name'] || 'unknown',
            size,
            theme,
            version: '0.1.0'
        });

        res.json({
            success: true,
            version: '0.1.0',
            data: {
                email,
                avatarUrl: avatarData.url,
                theme: avatarData.theme,
                themeConfig: avatarData.themeConfig,
                hasCustomAvatar,
                size
            }
        });
    } catch (error) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

/**
 * GET /api/v1/gravatar/profile/:email
 * Obtém perfil completo do Gravatar
 *
 * @param {string} email - Email do usuário
 * @returns {object} Perfil completo do Gravatar
 */
router.get('/profile/:email', async (req, res) => {
    try {
        const email = decodeURIComponent(req.params.email);
        const profile = await gravatar.getUserProfile(email);

        if (!profile) {
            return res.status(404).json({
                success: false,
                error: 'Perfil não encontrado'
            });
        }

        res.json({
            success: true,
            data: profile
        });
    } catch (error) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

/**
 * POST /api/v0/gravatar/avatars/batch
 * Obtém múltiplos avatares em uma única requisição com suporte a tema
 *
 * @body {string[]} emails - Array de emails
 * @body {object} options - Opções de avatar (size, default, theme)
 * @returns {object} Mapa de email -> dados do avatar
 */
router.post('/avatars/batch', async (req, res) => {
    try {
        const { emails, options = {} } = req.body;

        if (!Array.isArray(emails) || emails.length === 0) {
            return res.status(400).json({
                success: false,
                error: 'Array de emails é obrigatório'
            });
        }

        if (emails.length > 100) {
            return res.status(400).json({
                success: false,
                error: 'Máximo de 100 emails por requisição'
            });
        }

        const avatars = gravatar.getAvatarsBatch(emails, options);

        // Log analytics
        await gravatar.logAvatarUsage({
            endpoint: 'batch',
            count: emails.length,
            product: req.headers['x-product-name'] || 'unknown',
            theme: options.theme || 'light',
            version: '0.1.0'
        });

        res.json({
            success: true,
            version: '0.1.0',
            data: {
                avatars,
                count: emails.length,
                theme: options.theme || 'light'
            }
        });
    } catch (error) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

/**
 * GET /api/v1/gravatar/check/:email
 * Verifica se email possui Gravatar customizado
 *
 * @param {string} email - Email do usuário
 * @returns {object} { hasGravatar: boolean }
 */
router.get('/check/:email', async (req, res) => {
    try {
        const email = decodeURIComponent(req.params.email);
        const hasCustomAvatar = await gravatar.hasGravatar(email);

        res.json({
            success: true,
            data: {
                email,
                hasGravatar: hasCustomAvatar
            }
        });
    } catch (error) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

/**
 * GET /api/v0/gravatar/product/:product/:email
 * Obtém avatar formatado para produto específico com tema
 *
 * @param {string} product - Nome do produto (secreta, dashboard, pulse, archivus, barbara)
 * @param {string} email - Email do usuário
 * @query {string} theme - Tema (light, dark) - padrão: light
 * @returns {object} Dados formatados para o produto
 */
router.get('/product/:product/:email', async (req, res) => {
    try {
        const { product, email } = req.params;
        const decodedEmail = decodeURIComponent(email);
        const theme = req.query.theme || 'light';

        const validProducts = ['secreta', 'dashboard', 'pulse', 'archivus', 'barbara', 'shancrys'];
        if (!validProducts.includes(product.toLowerCase())) {
            return res.status(400).json({
                success: false,
                error: 'Produto inválido'
            });
        }

        const avatarData = await gravatar.getAvatarForProduct(decodedEmail, product, theme);

        res.json({
            success: true,
            version: '0.1.0',
            data: avatarData
        });
    } catch (error) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

/**
 * POST /api/v1/gravatar/profile/update
 * Atualiza perfil do Gravatar (requer OAuth token)
 *
 * @header {string} Authorization - Bearer token OAuth
 * @body {object} profileData - Dados do perfil a atualizar
 * @returns {object} Perfil atualizado
 */
router.post('/profile/update', async (req, res) => {
    try {
        const authHeader = req.headers.authorization;

        if (!authHeader || !authHeader.startsWith('Bearer ')) {
            return res.status(401).json({
                success: false,
                error: 'Token OAuth obrigatório'
            });
        }

        const accessToken = authHeader.substring(7);
        const profileData = req.body;

        const updatedProfile = await gravatar.updateProfile(accessToken, profileData);

        res.json({
            success: true,
            data: updatedProfile
        });
    } catch (error) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

/**
 * GET /api/v1/gravatar/hash/:email
 * Obtém hash MD5 do email (útil para frontend)
 *
 * @param {string} email - Email do usuário
 * @returns {object} { email, hash }
 */
router.get('/hash/:email', (req, res) => {
    try {
        const email = decodeURIComponent(req.params.email);
        const hash = gravatar.getEmailHash(email);

        res.json({
            success: true,
            data: {
                email,
                hash
            }
        });
    } catch (error) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

module.exports = router;
