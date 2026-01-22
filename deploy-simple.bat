@echo off
echo Deploying to existing repositories...
echo.

echo Landing Page - avilaops/avilainc
cd /d "%~dp0..\Landing-Pages\avila.inc"
mkdir dist 2>nul
xcopy public\* dist\ /E /I /Y >nul 2>&1
copy index.html dist\ >nul 2>&1
git add .
git commit -m "Deploy landing page - %date% %time%" 2>nul || echo No changes
git push origin main 2>nul || echo Push failed - check conflicts

echo.
echo Admin Dashboard - avilaops/manager
cd /d "%~dp0"
mkdir dist 2>nul
xcopy src\views\* dist\ /E /I /Y >nul 2>&1
xcopy src\public\* dist\ /E /I /Y >nul 2>&1
git add .
git commit -m "Deploy admin dashboard - %date% %time%" 2>nul || echo No changes
git push origin main 2>nul || echo Push failed - check conflicts

echo.
echo Done! Check GitHub Actions for deployment status.
pause