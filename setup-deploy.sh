#!/bin/bash

# 🚀 Script de Setup - Avila Manager Deploy
# Este script configura o ambiente de deploy profissional

echo "🚀 Configurando ambiente de deploy profissional..."

# Verificar se estamos em um repositório git
if ! git rev-parse --git-dir > /dev/null 2>&1; then
    echo "❌ Erro: Este não é um repositório Git"
    exit 1
fi

# Verificar branch atual
CURRENT_BRANCH=$(git branch --show-current)
echo "📍 Branch atual: $CURRENT_BRANCH"

# Criar branch develop se não existir
if ! git show-ref --verify --quiet refs/heads/develop; then
    echo "🌿 Criando branch 'develop'..."
    git checkout -b develop
    git push -u origin develop
    echo "✅ Branch 'develop' criada e configurada"
else
    echo "✅ Branch 'develop' já existe"
fi

# Verificar se main existe
if ! git show-ref --verify --quiet refs/heads/main; then
    echo "⚠️  Branch 'main' não encontrada. Renomeando master para main..."
    git branch -m master main
    git push -u origin main
    echo "✅ Branch renomeada para 'main'"
fi

# Verificar workflows
echo "🔍 Verificando workflows GitHub Actions..."
WORKFLOW_DIR=".github/workflows"

if [ -f "$WORKFLOW_DIR/staging_landing-web.yml" ]; then
    echo "✅ Workflow staging landing criado"
else
    echo "❌ Workflow staging landing não encontrado"
fi

if [ -f "$WORKFLOW_DIR/staging_manager-api.yml" ]; then
    echo "✅ Workflow staging API criado"
else
    echo "❌ Workflow staging API não encontrado"
fi

if [ -f "$WORKFLOW_DIR/main_landing-web.yml" ]; then
    echo "✅ Workflow produção landing configurado"
else
    echo "❌ Workflow produção landing não encontrado"
fi

if [ -f "$WORKFLOW_DIR/main_manager-api.yml" ]; then
    echo "✅ Workflow produção API configurado"
else
    echo "❌ Workflow produção API não encontrado"
fi

echo ""
echo "🎯 Próximos passos:"
echo ""
echo "1. 📝 Leia o guia completo: DEPLOY_GUIDE.md"
echo ""
echo "2. 🔐 Configure os secrets no GitHub:"
echo ""
echo "   📥 Publish Profiles (recomendado):"
echo "   - PUBLISH_PROFILE_API (Manager API)"
echo "   - PUBLISH_PROFILE_LANDING (Landing Page)"
echo ""
echo "   💡 Como obter: Azure Portal → App Service → Get publish profile"
echo ""
echo "   🔄 Ou use OIDC (mais complexo):"
echo "   - AZUREAPPSERVICE_CLIENTID_*"
echo "   - AZUREAPPSERVICE_TENANTID_*"
echo "   - AZUREAPPSERVICE_SUBSCRIPTIONID_*"
echo ""
echo "3. 🌿 Fluxo de trabalho:"
echo "   develop → staging (teste)"
echo "   main → produção (clientes)"
echo ""
echo "4. 🚀 Primeiro deploy de teste:"
echo "   git checkout develop"
echo "   git push origin develop"
echo ""
echo "✅ Setup concluído! Ambiente profissional configurado."