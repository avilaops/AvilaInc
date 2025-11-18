#!/usr/bin/env node
/**
 * Script para atualizar DNS do Cloudflare para GitHub Pages
 * Migração: Azure Static Web Apps → GitHub Pages (avilaops.github.io/AvilaInc)
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

const API_TOKEN = envVars.CLOUDFLARE_API || process.env.CLOUDFLARE_API_TOKEN || '90OIxY2lXTScwqUgg4g13smvLM4FSScIyDjitVfA';
const ZONE_ID = 'cd327658ec2b2196e1f5c085fd182c88';

// IPs do GitHub Pages
const GITHUB_PAGES_IPS = [
    '185.199.108.153',
    '185.199.109.153',
    '185.199.110.153',
    '185.199.111.153'
];

console.log('\n🔄 Migração DNS: Azure → GitHub Pages');
console.log('═'.repeat(60));

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
        console.log('\n📋 PASSO 1: Listar registros atuais...\n');
        
        const dnsRecords = await apiRequest(`/zones/${ZONE_ID}/dns_records`);
        
        if (!dnsRecords.success) {
            console.error('❌ Erro ao listar registros:', dnsRecords.errors);
            return;
        }

        // Encontrar registros A e CNAME do avila.inc
        const rootARecords = dnsRecords.result.filter(r => 
            r.type === 'A' && r.name === 'avila.inc'
        );
        
        const wwwCNAME = dnsRecords.result.find(r => 
            r.type === 'CNAME' && r.name === 'www.avila.inc'
        );

        console.log('Registros encontrados:');
        rootARecords.forEach(r => {
            console.log(`   • A @ → ${r.content} (ID: ${r.id})`);
        });
        if (wwwCNAME) {
            console.log(`   • CNAME www → ${wwwCNAME.content} (ID: ${wwwCNAME.id})`);
        }

        console.log('\n🗑️  PASSO 2: Deletar registros antigos (Azure)...\n');
        
        // Deletar registros A antigos
        for (const record of rootARecords) {
            console.log(`   Deletando A ${record.name} → ${record.content}...`);
            const result = await apiRequest(`/zones/${ZONE_ID}/dns_records/${record.id}`, 'DELETE');
            if (result.success) {
                console.log(`   ✅ Deletado`);
            } else {
                console.log(`   ❌ Erro:`, result.errors);
            }
        }

        // Deletar CNAME www se existir
        if (wwwCNAME && wwwCNAME.content.includes('azurestaticapps')) {
            console.log(`   Deletando CNAME www → ${wwwCNAME.content}...`);
            const result = await apiRequest(`/zones/${ZONE_ID}/dns_records/${wwwCNAME.id}`, 'DELETE');
            if (result.success) {
                console.log(`   ✅ Deletado`);
            } else {
                console.log(`   ❌ Erro:`, result.errors);
            }
        }

        console.log('\n➕ PASSO 3: Adicionar registros do GitHub Pages...\n');

        // Adicionar 4 registros A do GitHub Pages
        for (const ip of GITHUB_PAGES_IPS) {
            console.log(`   Adicionando A @ → ${ip}...`);
            const result = await apiRequest(`/zones/${ZONE_ID}/dns_records`, 'POST', {
                type: 'A',
                name: 'avila.inc',
                content: ip,
                ttl: 1,
                proxied: false,
                comment: 'GitHub Pages - avilaops.github.io/AvilaInc'
            });

            if (result.success) {
                console.log(`   ✅ Adicionado`);
            } else {
                console.log(`   ❌ Erro:`, result.errors);
            }
        }

        // Adicionar CNAME www
        console.log(`\n   Adicionando CNAME www → avilaops.github.io...`);
        const cnameResult = await apiRequest(`/zones/${ZONE_ID}/dns_records`, 'POST', {
            type: 'CNAME',
            name: 'www',
            content: 'avilaops.github.io',
            ttl: 1,
            proxied: false,
            comment: 'GitHub Pages - avilaops.github.io/AvilaInc'
        });

        if (cnameResult.success) {
            console.log(`   ✅ Adicionado`);
        } else {
            console.log(`   ❌ Erro:`, cnameResult.errors);
        }

        console.log('\n' + '═'.repeat(60));
        console.log('✅ Migração DNS concluída!');
        console.log('═'.repeat(60));

        console.log('\n📋 PRÓXIMOS PASSOS:\n');
        console.log('1. Aguarde propagação DNS (2-10 minutos)');
        console.log('   Teste: nslookup avila.inc\n');
        console.log('2. Configure domínio personalizado no GitHub:');
        console.log('   • Settings → Pages → Custom domain');
        console.log('   • Digite: avila.inc');
        console.log('   • Marque "Enforce HTTPS"\n');
        console.log('3. Crie arquivo CNAME no repositório:');
        console.log('   • Conteúdo: avila.inc\n');
        console.log('4. Aguarde certificado SSL (5-15 minutos)');
        console.log('   • GitHub Pages provisionará via Let\'s Encrypt\n');
        console.log('5. Acesse: https://avila.inc\n');

        console.log('═'.repeat(60));
        console.log('🎉 Site migrado com sucesso para GitHub Pages!\n');

    } catch (error) {
        console.error('\n❌ Erro:', error.message);
        process.exit(1);
    }
}

main();
