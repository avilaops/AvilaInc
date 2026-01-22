#!/bin/bash

# 🚀 Validação Completa - Sistema de Deploy
# Script para verificar se tudo está configurado corretamente

echo "🔍 Iniciando validação completa do sistema de deploy..."
echo ""

# 1. Verificar branches
echo "1️⃣ 📋 Verificando branches remotas..."
if git ls-remote --heads origin develop | grep -q develop; then
    echo "✅ Branch 'develop' existe no remoto"
else
    echo "❌ Branch 'develop' NÃO existe no remoto"
fi

if git ls-remote --heads origin main | grep -q main; then
    echo "✅ Branch 'main' existe no remoto"
else
    echo "❌ Branch 'main' NÃO existe no remoto"
fi
echo ""

# 2. Verificar workflows
echo "2️⃣ 🔄 Verificando workflows GitHub Actions..."
WORKFLOW_DIR=".github/workflows"

workflows=(
    "staging_landing-web.yml"
    "staging_manager-api.yml"
    "main_landing-web.yml"
    "main_manager-api.yml"
    "pr-checks.yml"
)

for workflow in "${workflows[@]}"; do
    if [ -f "$WORKFLOW_DIR/$workflow" ]; then
        echo "✅ $workflow existe"

        # Verificar gatilhos básicos
        if grep -q "branches:" "$WORKFLOW_DIR/$workflow"; then
            echo "   └─ ✅ Tem configuração de branches"
        else
            echo "   └─ ⚠️  Sem configuração de branches visível"
        fi

        # Verificar concurrency
        if grep -q "concurrency:" "$WORKFLOW_DIR/$workflow"; then
            echo "   └─ ✅ Tem controle de concorrência"
        else
            echo "   └─ ⚠️  Sem controle de concorrência"
        fi

    else
        echo "❌ $workflow NÃO encontrado"
    fi
done
echo ""

# 3. Verificar builds apontam para .csproj
echo "3️⃣ 🏗️ Verificando se builds apontam para .csproj correto..."

csproj_files=(
    "src/Landing/Landing.csproj"
    "src/Manager.Api/Manager.Api.csproj"
)

for csproj in "${csproj_files[@]}"; do
    if [ -f "$csproj" ]; then
        echo "✅ $csproj existe"

        # Verificar se workflows fazem referência correta
        workflow_refs=$(grep -r "$csproj" .github/workflows/ | wc -l)
        if [ "$workflow_refs" -gt 0 ]; then
            echo "   └─ ✅ Referenciado em workflows ($workflow_refs vezes)"
        else
            echo "   └─ ❌ NÃO referenciado em workflows"
        fi
    else
        echo "❌ $csproj NÃO encontrado"
    fi
done
echo ""

# 4. Verificar estrutura de diretórios
echo "4️⃣ 📁 Verificando estrutura de arquivos..."

required_files=(
    "DEPLOY_GUIDE.md"
    "FEATURE_EXAMPLE.md"
    "setup-deploy.bat"
    "setup-deploy.sh"
)

for file in "${required_files[@]}"; do
    if [ -f "$file" ]; then
        echo "✅ $file existe"
    else
        echo "❌ $file NÃO encontrado"
    fi
done
echo ""

# 5. Verificar se estamos na branch correta
echo "5️⃣ 🌿 Verificando branch atual..."
current_branch=$(git branch --show-current)
echo "📍 Branch atual: $current_branch"

if [ "$current_branch" = "develop" ]; then
    echo "✅ Estamos na branch 'develop' (staging)"
elif [ "$current_branch" = "main" ]; then
    echo "✅ Estamos na branch 'main' (produção)"
else
    echo "⚠️  Estamos na branch '$current_branch' (feature branch)"
fi
echo ""

# 6. Verificar se há mudanças não commitadas
echo "6️⃣ 💾 Verificando status do Git..."
if [ -z "$(git status --porcelain)" ]; then
    echo "✅ Working directory limpo (sem mudanças não commitadas)"
else
    echo "⚠️  Há mudanças não commitadas:"
    git status --short
fi
echo ""

# 7. Resumo final
echo "🎯 RESUMO DA VALIDAÇÃO:"
echo ""

errors=0
warnings=0

# Contar erros e warnings
if ! git ls-remote --heads origin develop | grep -q develop; then ((errors++)); fi
if ! git ls-remote --heads origin main | grep -q main; then ((errors++)); fi

for workflow in "${workflows[@]}"; do
    if [ ! -f "$WORKFLOW_DIR/$workflow" ]; then ((errors++)); fi
done

for csproj in "${csproj_files[@]}"; do
    if [ ! -f "$csproj" ]; then ((errors++)); fi
    workflow_refs=$(grep -r "$csproj" .github/workflows/ | wc -l)
    if [ "$workflow_refs" -eq 0 ]; then ((errors++)); fi
done

for file in "${required_files[@]}"; do
    if [ ! -f "$file" ]; then ((errors++)); fi
done

if [ -n "$(git status --porcelain)" ]; then ((warnings++)); fi

echo "❌ Erros críticos: $errors"
echo "⚠️  Avisos: $warnings"
echo ""

if [ $errors -eq 0 ]; then
    echo "🎉 SISTEMA VALIDADO COM SUCESSO!"
    echo ""
    echo "✅ Tudo está configurado corretamente"
    echo "🚀 Pode fazer deploy frequente sem medo"
    echo ""
    echo "📋 PRÓXIMOS PASSOS:"
    echo "1. Configure os secrets no GitHub se ainda não configurou"
    echo "2. Teste um push para develop para validar staging"
    echo "3. Depois de validar, faça merge para main"
    echo ""
else
    echo "⚠️  SISTEMA PRECISA DE AJUSTES!"
    echo ""
    echo "🔧 Corrija os erros acima antes de fazer deploy"
    echo "📖 Consulte DEPLOY_GUIDE.md para orientações"
    echo ""
fi