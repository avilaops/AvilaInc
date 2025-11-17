#!/usr/bin/env node
/**
 * Script para gerenciar DNS do Cloudflare
 * Uso: node scripts/cloudflare-dns.js
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
const ACCOUNT_ID = envVars.CLOUDFLARE_ACCOUNT_ID || '85b914dcdebf6fa2fce7b819bc542294';

console.log('\n🌐 Cloudflare DNS Manager\n' + '═'.repeat(50));

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
        // 1. Verificar token
        console.log('\n📋 Verificando token...');
        const verify = await apiRequest('/user/tokens/verify');

        if (!verify.success) {
            console.error('❌ Token inválido!');
            console.error('Erros:', verify.errors);
            process.exit(1);
        }

        console.log('✅ Token válido!');
        console.log(`   Status: ${verify.result.status}`);

        // 2. Listar zonas
        console.log('\n📋 Listando domínios...');
        const zones = await apiRequest('/zones');

        if (!zones.success) {
            console.error('❌ Erro ao listar zonas:', zones.errors);
            process.exit(1);
        }

        console.log(`✅ ${zones.result.length} domínio(s) encontrado(s):\n`);

        let avilaZone = null;
        zones.result.forEach(zone => {
            const status = zone.status === 'active' ? '🟢' : '🟡';
            console.log(`   ${status} ${zone.name}`);
            console.log(`      ID: ${zone.id}`);
            console.log(`      Status: ${zone.status}`);
            console.log(`      Name Servers: ${zone.name_servers.join(', ')}\n`);

            if (zone.name === 'avila.inc') {
                avilaZone = zone;
            }
        });

        if (!avilaZone) {
            console.log('⚠️  Domínio avila.inc não encontrado!');
            return;
        }

        console.log('🎯 Focando em: avila.inc\n');

        // 3. Listar registros DNS
        console.log('📋 Registros DNS atuais:\n');
        const dns = await apiRequest(`/zones/${avilaZone.id}/dns_records`);

        if (!dns.success) {
            console.error('❌ Erro ao listar DNS:', dns.errors);
            return;
        }

        const records = dns.result;
        console.log('Tipo     Nome                            Conteúdo                                         Proxy');
        console.log('─'.repeat(120));

        records.forEach(record => {
            const type = record.type.padEnd(8);
            const name = record.name.padEnd(30);
            const content = record.content.substring(0, 45).padEnd(45);
            const proxied = record.proxied ? '🟠 Proxied' : '🔘 DNS only';
            console.log(`${type} ${name} ${content} ${proxied}`);
        });

        // 4. Verificar configuração
        console.log('\n📊 Análise de Configuração:\n');

        const rootA = records.find(r => r.type === 'A' && r.name === 'avila.inc');
        const rootCNAME = records.find(r => r.type === 'CNAME' && r.name === 'avila.inc');
        const wwwCNAME = records.find(r => r.type === 'CNAME' && r.name === 'www.avila.inc');
        const txtAsuid = records.find(r => r.type === 'TXT' && r.name.includes('asuid'));

        if (rootA && rootA.content === '20.65.18.151') {
            console.log('✅ Registro A configurado corretamente');
            console.log(`   avila.inc → ${rootA.content}`);
        } else if (rootCNAME) {
            console.log('⚠️  Registro raiz é CNAME (deveria ser A)');
            console.log(`   ${rootCNAME.name} → ${rootCNAME.content}`);
            console.log('\n💡 Ação necessária: Deletar CNAME e criar registro A:');
            console.log(`   wrangler dns delete ${rootCNAME.id}`);
            console.log('   Depois criar: A @ → 20.65.18.151');
        } else {
            console.log('❌ Registro A não encontrado');
            console.log('\n💡 Adicione: Type=A, Name=@, Value=20.65.18.151');
        }

        if (wwwCNAME && wwwCNAME.content.includes('azurestaticapps')) {
            console.log('\n✅ Registro CNAME www configurado');
            console.log(`   ${wwwCNAME.name} → ${wwwCNAME.content}`);
        } else {
            console.log('\n⚠️  CNAME www não encontrado ou incorreto');
        }

        if (txtAsuid) {
            console.log('\n✅ TXT de validação Azure encontrado');
            console.log(`   ${txtAsuid.name}`);
        } else {
            console.log('\n⚠️  TXT de validação Azure não encontrado');
            console.log('   Necessário para validar domínio no Azure');
        }

        // 5. Próximos passos
        console.log('\n' + '═'.repeat(50));
        console.log('📝 Próximos Passos:\n');

        if (rootCNAME) {
            console.log('1. ❗ Deletar CNAME avila.inc (conflita com A)');
            console.log(`   ID para deletar: ${rootCNAME.id}`);
            console.log('2. ➕ Adicionar registro A: @ → 20.65.18.151');
        } else if (!rootA) {
            console.log('1. ➕ Adicionar registro A: @ → 20.65.18.151');
        }

        console.log('3. 🌐 Ir ao Azure Portal: https://portal.azure.com');
        console.log('4. ➕ Custom domains → Add → avila.inc');
        console.log('5. 📋 Copiar código TXT de validação');
        console.log('6. ➕ Adicionar TXT no Cloudflare');
        console.log('7. ✅ Validar no Azure');
        console.log('8. ⏰ Aguardar 5-30min para SSL\n');

        // Salvar Zone ID no .env
        if (avilaZone) {
            console.log(`💡 Zone ID: ${avilaZone.id}`);
            console.log('   Adicione ao .env: CLOUDFLARE_ZONE_ID=' + avilaZone.id + '\n');
        }

    } catch (error) {
        console.error('\n❌ Erro:', error.message);
        process.exit(1);
    }
}

main();
