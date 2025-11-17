#!/usr/bin/env node
/**
 * Script para adicionar registros DNS necessários no avila.inc
 */

const https = require('https');
const fs = require('fs');
const path = require('path');

// Carregar .env
const envPath = path.join(__dirname, '..', '.env');
const envVars = {};

if (fs.existsSync(envPath)) {
    const envContent = fs.readFileSync(envPath, 'utf8');
    envContent.split('\n').forEach(line => {
        const match = line.match(/^([^#=]+)=(.*)$/);
        if (match) {
            envVars[match[1].trim()] = match[2].trim();
        }
    });
}

const API_TOKEN = envVars.CLOUDFLARE_API || envVars.CLOUDFLARE_API_TOKEN || process.env.CLOUDFLARE_API_TOKEN || '90OIxY2lXTScwqUgg4g13smvLM4FSScIyDjitVfA';
const ZONE_ID = 'cd327658ec2b2196e1f5c085fd182c88';

console.log('\n🚀 Adicionar Registros DNS - avila.inc');
console.log('═'.repeat(60));

// Função para fazer requisições
function apiRequest(path, method = 'GET', data = null) {
    return new Promise((resolve, reject) => {
        const options = {
            hostname: 'api.cloudflare.com',
            path: `/client/v4${path}`,
            method: method,
            headers: {
                'Authorization': `Bearer ${API_TOKEN}`,
                'Content-Type': 'application/json'
            }
        };

        const req = https.request(options, (res) => {
            let body = '';
            res.on('data', chunk => body += chunk);
            res.on('end', () => {
                try {
                    resolve(JSON.parse(body));
                } catch (e) {
                    reject(e);
                }
            });
        });

        req.on('error', reject);

        if (data) {
            req.write(JSON.stringify(data));
        }

        req.end();
    });
}

async function main() {
    try {
        console.log('\n✨ Adicionando registros DNS...\n');

        // 1. Adicionar registro A para domínio raiz
        console.log('1️⃣  Adicionando registro A @ → 20.65.18.151...');
        const aRecord = await apiRequest(`/zones/${ZONE_ID}/dns_records`, 'POST', {
            type: 'A',
            name: 'avila.inc',
            content: '20.65.18.151',
            ttl: 1,
            proxied: false,
            comment: 'Azure Static Web App - salmon-island'
        });

        if (aRecord.success) {
            console.log('   ✅ Registro A adicionado com sucesso!');
        } else {
            console.log('   ❌ Erro:', aRecord.errors);
        }

        // 2. Adicionar registro CNAME para www
        console.log('\n2️⃣  Adicionando registro CNAME www → salmon-island-0f049391e.3.azurestaticapps.net...');
        const cnameRecord = await apiRequest(`/zones/${ZONE_ID}/dns_records`, 'POST', {
            type: 'CNAME',
            name: 'www',
            content: 'salmon-island-0f049391e.3.azurestaticapps.net',
            ttl: 1,
            proxied: false,
            comment: 'Azure Static Web App - salmon-island'
        });

        if (cnameRecord.success) {
            console.log('   ✅ Registro CNAME adicionado com sucesso!');
        } else {
            console.log('   ❌ Erro:', cnameRecord.errors);
        }

        console.log('\n' + '═'.repeat(60));
        console.log('✅ Registros DNS configurados!');
        console.log('═'.repeat(60));

        console.log('\n📋 Próximos passos:');
        console.log('\n1. Aguarde propagação DNS (2-5 minutos)');
        console.log('   Teste: nslookup avila.inc');
        console.log('\n2. Configure domínio personalizado no Azure Portal:');
        console.log('   • Acesse: https://portal.azure.com');
        console.log('   • Static Web Apps → salmon-island-0f049391e');
        console.log('   • Custom domains → + Add');
        console.log('   • Digite: avila.inc');
        console.log('   • Copie o código TXT de validação');
        console.log('\n3. Adicione registro TXT no Cloudflare:');
        console.log('   • Type: TXT');
        console.log('   • Name: asuid (ou o que o Azure indicar)');
        console.log('   • Content: [código do Azure]');
        console.log('\n4. No Azure, clique em "Validate" e depois "Add"');
        console.log('\n5. Aguarde SSL (5-30 minutos)');
        console.log('   • Certificado Let\'s Encrypt automático');
        console.log('   • https://avila.inc funcionará!\n');

        console.log('═'.repeat(60));
        console.log('🎉 Configuração DNS concluída!\n');

    } catch (error) {
        console.error('\n❌ Erro:', error.message);
        process.exit(1);
    }
}

main();
