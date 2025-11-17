#!/usr/bin/env node
/**
 * Script para limpar e configurar DNS do avila.inc
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

console.log('\n🔍 Análise de Registros DNS - avila.inc');
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
        // Listar todos os registros
        console.log('\n📋 Listando TODOS os registros DNS...\n');
        const dns = await apiRequest(`/zones/${ZONE_ID}/dns_records`);

        if (!dns.success) {
            console.error('❌ Erro:', dns.errors);
            return;
        }

        const records = dns.result;

        // Categorizar registros
        const rootRecords = [];
        const wwwRecords = [];
        const subdomainRecords = [];
        const validationRecords = [];
        const emailRecords = [];

        records.forEach(record => {
            if (record.name === 'avila.inc') {
                if (record.type === 'MX') {
                    emailRecords.push(record);
                } else {
                    rootRecords.push(record);
                }
            } else if (record.name === 'www.avila.inc') {
                wwwRecords.push(record);
            } else if (record.type === 'TXT' && (record.name.includes('asuid') || record.name.includes('_domainkey'))) {
                validationRecords.push(record);
            } else {
                subdomainRecords.push(record);
            }
        });

        // Exibir análise
        console.log('═'.repeat(60));
        console.log('📊 ANÁLISE DE REGISTROS');
        console.log('═'.repeat(60));

        // Registros do domínio raiz
        console.log('\n🏠 DOMÍNIO RAIZ (avila.inc):');
        if (rootRecords.length === 0) {
            console.log('   ✅ Nenhum registro (pronto para adicionar A)');
        } else {
            rootRecords.forEach(r => {
                console.log(`   ⚠️  ${r.type} → ${r.content}`);
                console.log(`      ID: ${r.id}`);
                console.log(`      ❗ CONFLITO: Impede adicionar registro A`);
            });
        }

        // Registros www
        console.log('\n🌐 WWW (www.avila.inc):');
        if (wwwRecords.length === 0) {
            console.log('   ✅ Nenhum registro (pronto para adicionar CNAME)');
        } else {
            wwwRecords.forEach(r => {
                console.log(`   ℹ️  ${r.type} → ${r.content}`);
                console.log(`      ID: ${r.id}`);
            });
        }

        // Email
        console.log('\n📧 EMAIL (MX Records):');
        emailRecords.forEach(r => {
            console.log(`   ✅ ${r.type} (${r.priority}) → ${r.content}`);
            console.log(`      Manter para email funcionar`);
        });

        // Subdomínios
        console.log('\n🔗 SUBDOMÍNIOS:');
        subdomainRecords.forEach(r => {
            console.log(`   ℹ️  ${r.name} (${r.type}) → ${r.content.substring(0, 50)}`);
        });

        // Validação
        console.log('\n🔐 REGISTROS DE VALIDAÇÃO:');
        if (validationRecords.length === 0) {
            console.log('   ⚠️  Nenhum (será necessário adicionar TXT asuid do Azure)');
        } else {
            validationRecords.forEach(r => {
                console.log(`   ℹ️  ${r.name} (${r.type})`);
            });
        }

        // Ações recomendadas
        console.log('\n' + '═'.repeat(60));
        console.log('⚡ AÇÕES NECESSÁRIAS');
        console.log('═'.repeat(60));

        const toDelete = [];

        if (rootRecords.length > 0) {
            console.log('\n❌ DELETAR (conflitam com registro A):');
            rootRecords.forEach(r => {
                console.log(`   • ${r.type} ${r.name} → ${r.content}`);
                console.log(`     ID: ${r.id}`);
                toDelete.push(r);
            });
        }

        console.log('\n✅ ADICIONAR:');
        if (rootRecords.length === 0) {
            console.log('   • A @ → 20.65.18.151');
        }
        if (wwwRecords.length === 0) {
            console.log('   • CNAME www → salmon-island-0f049391e.3.azurestaticapps.net');
        }
        console.log('   • TXT asuid → [código do Azure Portal]');

        console.log('\n✅ MANTER:');
        console.log(`   • ${emailRecords.length} registros MX (email)`);
        console.log(`   • ${subdomainRecords.length} subdomínios`);

        // Confirmar deleção
        if (toDelete.length > 0) {
            console.log('\n' + '═'.repeat(60));
            console.log('🗑️  CONFIRMAR DELEÇÃO');
            console.log('═'.repeat(60));

            console.log('\nPara deletar automaticamente, execute:');
            console.log('node scripts/cloudflare-cleanup.js --delete\n');

            // Se argumento --delete foi passado
            if (process.argv.includes('--delete')) {
                console.log('🗑️  Deletando registros conflitantes...\n');

                for (const record of toDelete) {
                    console.log(`   Deletando: ${record.type} ${record.name}...`);
                    const result = await apiRequest(`/zones/${ZONE_ID}/dns_records/${record.id}`, 'DELETE');

                    if (result.success) {
                        console.log(`   ✅ Deletado: ${record.name}`);
                    } else {
                        console.log(`   ❌ Erro ao deletar: ${result.errors}`);
                    }
                }

                console.log('\n✅ Limpeza concluída!');
                console.log('\n📋 Agora adicione os registros necessários:');
                console.log('   1. A @ → 20.65.18.151');
                console.log('   2. CNAME www → salmon-island-0f049391e.3.azurestaticapps.net');
                console.log('   3. Configure no Azure Portal');
                console.log('   4. Adicione TXT de validação');
            }
        } else {
            console.log('\n✅ Nenhuma limpeza necessária! Domínio raiz está limpo.');
            console.log('\n📋 Próximo passo: Adicionar registros no Cloudflare:');
            console.log('   https://dash.cloudflare.com/');
        }

        console.log('\n' + '═'.repeat(60));
        console.log('✨ Análise concluída!\n');

    } catch (error) {
        console.error('\n❌ Erro:', error.message);
        process.exit(1);
    }
}

main();
