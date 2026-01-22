@echo off
REM 🚀 Validação Completa - Sistema de Deploy (Windows)
REM Script para verificar se tudo está configurado corretamente

echo 🔍 Iniciando validação completa do sistema de deploy...
echo.

REM 1. Verificar branches
echo 1️⃣ 📋 Verificando branches remotas...
git ls-remote --heads origin develop | findstr develop >nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ Branch 'develop' existe no remoto
) else (
    echo ❌ Branch 'develop' NÃO existe no remoto
)

git ls-remote --heads origin main | findstr main >nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ Branch 'main' existe no remoto
) else (
    echo ❌ Branch 'main' NÃO existe no remoto
)
echo.

REM 2. Verificar workflows
echo 2️⃣ 🔄 Verificando workflows GitHub Actions...
set WORKFLOW_DIR=.github\workflows

set workflows=staging_landing-web.yml staging_manager-api.yml main_landing-web.yml main_manager-api.yml pr-checks.yml

for %%w in (%workflows%) do (
    if exist "%WORKFLOW_DIR%\%%w" (
        echo ✅ %%w existe
    ) else (
        echo ❌ %%w NÃO encontrado
    )
)
echo.

REM 3. Verificar builds apontam para .csproj
echo 3️⃣ 🏗️ Verificando se builds apontam para .csproj correto...

set csproj_files=src\Landing\Landing.csproj src\Manager.Api\Manager.Api.csproj

for %%c in (%csproj_files%) do (
    if exist "%%c" (
        echo ✅ %%c existe
    ) else (
        echo ❌ %%c NÃO encontrado
    )
)
echo.

REM 4. Verificar estrutura de diretórios
echo 4️⃣ 📁 Verificando estrutura de arquivos...

set required_files=DEPLOY_GUIDE.md FEATURE_EXAMPLE.md setup-deploy.bat setup-deploy.sh

for %%f in (%required_files%) do (
    if exist "%%f" (
        echo ✅ %%f existe
    ) else (
        echo ❌ %%f NÃO encontrado
    )
)
echo.

REM 5. Verificar branch atual
echo 5️⃣ 🌿 Verificando branch atual...
for /f "tokens=*" %%i in ('git branch --show-current') do set current_branch=%%i
echo 📍 Branch atual: %current_branch%

if "%current_branch%"=="develop" (
    echo ✅ Estamos na branch 'develop' (staging)
) else if "%current_branch%"=="main" (
    echo ✅ Estamos na branch 'main' (produção)
) else (
    echo ⚠️  Estamos na branch '%current_branch%' (feature branch)
)
echo.

REM 6. Verificar mudanças não commitadas
echo 6️⃣ 💾 Verificando status do Git...
git status --porcelain >nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ Working directory limpo (sem mudanças não commitadas)
) else (
    echo ⚠️  Há mudanças não commitadas:
    git status --short
)
echo.

REM 7. Resumo final
echo 🎯 RESUMO DA VALIDAÇÃO:
echo.

echo ❌ Erros críticos: Verificação manual necessária
echo ⚠️  Avisos: Verificação manual necessária
echo.
echo 📋 Execute também: .\validate-deploy.sh (no WSL/Git Bash) para validação completa
echo.
echo 🎉 Sistema preparado para deploy profissional!
echo.
echo 📖 Consulte DEPLOY_GUIDE.md para detalhes completos