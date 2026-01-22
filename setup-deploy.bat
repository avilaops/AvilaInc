@echo off
REM 🚀 Script de Setup - Avila Manager Deploy (Windows)
REM Este script configura o ambiente de deploy profissional

echo 🚀 Configurando ambiente de deploy profissional...

REM Verificar se estamos em um repositório git
git rev-parse --git-dir >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Erro: Este não é um repositório Git
    exit /b 1
)

REM Verificar branch atual
for /f "tokens=*" %%i in ('git branch --show-current') do set CURRENT_BRANCH=%%i
echo 📍 Branch atual: %CURRENT_BRANCH%

REM Criar branch develop se não existir
git show-ref --verify --quiet refs/heads/develop
if %errorlevel% neq 0 (
    echo 🌿 Criando branch 'develop'...
    git checkout -b develop
    git push -u origin develop
    echo ✅ Branch 'develop' criada e configurada
) else (
    echo ✅ Branch 'develop' já existe
)

REM Verificar se main existe
git show-ref --verify --quiet refs/heads/main
if %errorlevel% neq 0 (
    echo ⚠️  Branch 'main' não encontrada. Renomeando master para main...
    git branch -m master main
    git push -u origin main
    echo ✅ Branch renomeada para 'main'
)

REM Verificar workflows
echo 🔍 Verificando workflows GitHub Actions...
if exist ".github\workflows\staging_landing-web.yml" (
    echo ✅ Workflow staging landing criado
) else (
    echo ❌ Workflow staging landing não encontrado
)

if exist ".github\workflows\staging_manager-api.yml" (
    echo ✅ Workflow staging API criado
) else (
    echo ❌ Workflow staging API não encontrado
)

if exist ".github\workflows\main_landing-web.yml" (
    echo ✅ Workflow produção landing configurado
) else (
    echo ❌ Workflow produção landing não encontrado
)

if exist ".github\workflows\main_manager-api.yml" (
    echo ✅ Workflow produção API configurado
) else (
    echo ❌ Workflow produção API não encontrado
)

echo.
echo 🎯 Próximos passos:
echo.
echo 1. 📝 Leia o guia completo: DEPLOY_GUIDE.md
echo.
echo 2. 🔐 Configure os secrets no GitHub:
echo    - AZUREAPPSERVICE_CLIENTID_*
echo    - AZUREAPPSERVICE_TENANTID_*
echo    - AZUREAPPSERVICE_SUBSCRIPTIONID_*
echo    - PUBLISH_PROFILE_API
echo.
echo 3. 🌿 Fluxo de trabalho:
echo    develop → staging (teste)
echo    main → produção (clientes)
echo.
echo 4. 🚀 Primeiro deploy de teste:
echo    git checkout develop
echo    git push origin develop
echo.
echo ✅ Setup concluído! Ambiente profissional configurado.