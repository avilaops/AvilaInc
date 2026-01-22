@echo off
echo ========================================
echo    AVILA INC - DEPLOY SCRIPT
echo ========================================
echo.

echo Passo 1: Preparando Landing Page...
echo.

cd /d "%~dp0..\Landing-Pages\avila.inc"

if not exist ".git" (
    echo Inicializando Git para landing page...
    git init
    git remote add origin https://github.com/avilaops/avila-inc-landing.git
)

echo Copiando arquivos para dist...
if not exist "dist" mkdir dist
xcopy public\* dist\ /E /I /Y
copy index.html dist\ >nul 2>&1

echo.
echo Passo 2: Preparando Admin Dashboard...
echo.

cd /d "%~dp0"

if not exist ".git" (
    echo Inicializando Git para admin...
    git init
    git remote add origin https://github.com/avilaops/avila-inc-admin.git
)

echo Copiando arquivos para dist...
if not exist "dist" mkdir dist
xcopy src\views\* dist\ /E /I /Y
xcopy src\public\* dist\ /E /I /Y 2>nul

echo.
echo ========================================
echo    INSTRUCOES PARA DEPLOY:
echo ========================================
echo.
echo 1. Crie dois repositorios no GitHub:
echo    - avilaops/avilainc (landing page)
echo    - avilaops/manager (admin dashboard)
echo.
echo 2. Configure dominios:
echo    - avila-inc-landing: avila.inc
echo    - avila-inc-admin: manager.avila.inc
echo.
echo 3. Push dos arquivos:
echo.
echo    # Landing Page
echo    cd ../Landing-Pages/avila.inc
echo    git add .
echo    git commit -m "Deploy landing page"
echo    git push -u origin main
echo.
echo    # Admin Dashboard
echo    cd ../../Admin
echo    git add .
echo    git commit -m "Deploy admin dashboard"
echo    git push -u origin main
echo.
echo 4. GitHub Actions fara deploy automatico!
echo.
echo URLs finais:
echo - https://avila.inc (landing)
echo - https://manager.avila.inc (admin)
echo.
pause