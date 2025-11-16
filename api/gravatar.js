/**
 * Gravatar API Integration v0.1.0
 * Integração com Gravatar para avatares de usuários
 * API Key: gk-ozHmHXVf-TMBrfDAk69r3ejAkSM8CgwdAqNuhXNrpBqmsSix7Tpv08FL8CUs9
 * 
 * @version 0.1.0
 * @date 2025-11-16
 */

const crypto = require('crypto');

const GRAVATAR_CONFIG = {
    version: '0.1.0',
    apiKey: 'gk-ozHmHXVf-TMBrfDAk69r3ejAkSM8CgwdAqNuhXNrpBqmsSix7Tpv08FL8CUs9',
    baseUrl: 'https://api.gravatar.com/v3',
    imageUrl: 'https://www.gravatar.com/avatar',
    defaultSize: 200,
    rating: 'pg', // g, pg, r, x
    themes: {
        light: {
            borderColor: '#e5e5e5',
            backgroundColor: '#ffffff',
            textColor: '#1a1a1a'
        },
        dark: {
            borderColor: '#333333',
            backgroundColor: '#1a1a1a',
            textColor: '#ffffff'
        }
    }
};

/**
 * Gera hash MD5 do email para Gravatar
 * @param {string} email - Email do usuário
 * @returns {string} Hash MD5
 */
function getEmailHash(email) {
    if (!email) return '';
    return crypto
        .createHash('md5')
        .update(email.toLowerCase().trim())
        .digest('hex');
}

/**
 * GET: Obtém URL do avatar do usuário com suporte a tema
 * @param {string} email - Email do usuário
 * @param {object} options - Opções (size, default, rating, theme)
 * @returns {object} URL do avatar e configurações de tema
 */
function getAvatarUrl(email, options = {}) {
    const hash = getEmailHash(email);
    const size = options.size || GRAVATAR_CONFIG.defaultSize;
    const defaultImage = options.default || 'mp'; // mp, identicon, monsterid, wavatar, retro, robohash
    const rating = options.rating || GRAVATAR_CONFIG.rating;
    const theme = options.theme || 'light'; // light, dark

    const params = new URLSearchParams({
        s: size,
        d: defaultImage,
        r: rating
    });

    const url = `${GRAVATAR_CONFIG.imageUrl}/${hash}?${params.toString()}`;
    const themeConfig = GRAVATAR_CONFIG.themes[theme] || GRAVATAR_CONFIG.themes.light;

    return {
        url,
        theme,
        themeConfig,
        size,
        hash
    };
}

/**
 * GET: Obtém perfil completo do usuário via API
 * @param {string} email - Email do usuário
 * @returns {Promise<object>} Dados do perfil
 */
async function getUserProfile(email) {
    const hash = getEmailHash(email);

    try {
        const response = await fetch(`${GRAVATAR_CONFIG.baseUrl}/profiles/${hash}`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${GRAVATAR_CONFIG.apiKey}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error(`Gravatar API error: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error('Erro ao buscar perfil Gravatar:', error);
        return null;
    }
}

/**
 * GET: Verifica se email tem Gravatar
 * @param {string} email - Email do usuário
 * @returns {Promise<boolean>} True se tem Gravatar
 */
async function hasGravatar(email) {
    const hash = getEmailHash(email);

    try {
        const response = await fetch(`${GRAVATAR_CONFIG.imageUrl}/${hash}?d=404`, {
            method: 'HEAD'
        });

        return response.status === 200;
    } catch (error) {
        return false;
    }
}

/**
 * POST: Atualiza ou cria perfil no Gravatar (requer autenticação OAuth)
 * Nota: Esta função requer que o usuário tenha autorizado via OAuth
 * @param {string} accessToken - Token OAuth do usuário
 * @param {object} profileData - Dados do perfil
 * @returns {Promise<object>} Resposta da API
 */
async function updateProfile(accessToken, profileData) {
    try {
        const response = await fetch(`${GRAVATAR_CONFIG.baseUrl}/profiles/me`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${accessToken}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(profileData)
        });

        if (!response.ok) {
            throw new Error(`Gravatar API error: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error('Erro ao atualizar perfil Gravatar:', error);
        throw error;
    }
}

/**
 * GET: Busca múltiplos avatares em batch com suporte a tema
 * @param {string[]} emails - Array de emails
 * @param {object} options - Opções de avatar (size, default, theme)
 * @returns {object} Mapa de email -> dados do avatar
 */
function getAvatarsBatch(emails, options = {}) {
    return emails.reduce((acc, email) => {
        acc[email] = getAvatarUrl(email, options);
        return acc;
    }, {});
}

/**
 * GET: Obtém dados estruturados para o produto específico com tema
 * @param {string} email - Email do usuário
 * @param {string} product - Nome do produto (secreta, dashboard, pulse, etc)
 * @param {string} theme - Tema (light, dark)
 * @returns {Promise<object>} Dados formatados para o produto
 */
async function getAvatarForProduct(email, product, theme = 'light') {
    const avatarData = getAvatarUrl(email, { size: 80, theme });
    const hasAvatar = await hasGravatar(email);

    return {
        email,
        product,
        avatarUrl: avatarData.url,
        theme,
        themeConfig: avatarData.themeConfig,
        hasCustomAvatar: hasAvatar,
        timestamp: new Date().toISOString(),
        fallback: hasAvatar ? null : 'initials',
        version: GRAVATAR_CONFIG.version
    };
}

/**
 * POST: Registra uso do avatar (analytics interno)
 * @param {object} usageData - Dados de uso
 * @returns {Promise<void>}
 */
async function logAvatarUsage(usageData) {
    // Integração com analytics interno da Ávila Inc
    try {
        await fetch('https://api.avila.inc/v1/analytics/gravatar', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-API-Key': process.env.AVILA_API_KEY
            },
            body: JSON.stringify({
                ...usageData,
                timestamp: new Date().toISOString(),
                source: 'gravatar-integration'
            })
        });
    } catch (error) {
        console.error('Erro ao registrar uso:', error);
    }
}

// Exporta funções e configuração
module.exports = {
    GRAVATAR_CONFIG,
    getAvatarUrl,
    getUserProfile,
    hasGravatar,
    updateProfile,
    getAvatarsBatch,
    getAvatarForProduct,
    logAvatarUsage,
    getEmailHash
};

// Para uso direto no browser (frontend)
if (typeof window !== 'undefined') {
    window.GravatarAPI = {
        version: GRAVATAR_CONFIG.version,
        getAvatarUrl,
        getEmailHash,
        getAvatarsBatch,
        themes: GRAVATAR_CONFIG.themes
    };
}
