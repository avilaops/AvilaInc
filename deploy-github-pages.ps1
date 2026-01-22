param(
    [switch]$DeployLanding,
    [switch]$DeployAdmin,
    [switch]$DeployBoth
)

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "    AVILA INC - GITHUB PAGES DEPLOY" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$rootPath = Split-Path -Parent $PSScriptRoot
$landingPath = Join-Path $rootPath "Landing-Pages\avila.inc"
$adminPath = $PSScriptRoot

function Deploy-Landing {
    Write-Host "🚀 Deploying Landing Page..." -ForegroundColor Green
    Write-Host ""

    Set-Location $landingPath

    # Verificar se repositório existe
    if (!(Test-Path ".git")) {
        Write-Host "📝 Inicializando repositório Git..." -ForegroundColor Yellow
        git init
        git remote add origin https://github.com/avilaops/avilainc.git
    }

    # Build
    Write-Host "🔨 Fazendo build..." -ForegroundColor Yellow
    if (!(Test-Path "dist")) { New-Item -ItemType Directory -Path "dist" | Out-Null }
    Copy-Item "public\*" "dist\" -Recurse -Force
    if (Test-Path "index.html") { Copy-Item "index.html" "dist\" -Force }

    # Commit e push
    Write-Host "📤 Fazendo commit e push..." -ForegroundColor Yellow
    git add .
    git commit -m "Deploy landing page - $(Get-Date -Format 'yyyy-MM-dd HH:mm')" 2>$null
    git push -u origin main

    Write-Host "✅ Landing page deployed!" -ForegroundColor Green
    Write-Host "   URL: https://avila.inc" -ForegroundColor Cyan
    Write-Host ""
}

function Deploy-Admin {
    Write-Host "🚀 Deploying Admin Dashboard..." -ForegroundColor Green
    Write-Host ""

    Set-Location $adminPath

    # Verificar se repositório existe
    if (!(Test-Path ".git")) {
        Write-Host "📝 Inicializando repositório Git..." -ForegroundColor Yellow
        git init
        git remote add origin https://github.com/avilaops/manager.git
    }

    # Build
    Write-Host "🔨 Fazendo build..." -ForegroundColor Yellow
    if (!(Test-Path "dist")) { New-Item -ItemType Directory -Path "dist" | Out-Null }
    Copy-Item "src\views\*" "dist\" -Recurse -Force
    if (Test-Path "src\public") { Copy-Item "src\public\*" "dist\" -Recurse -Force }

    # Commit e push
    Write-Host "📤 Fazendo commit e push..." -ForegroundColor Yellow
    git add .
    git commit -m "Deploy admin dashboard - $(Get-Date -Format 'yyyy-MM-dd HH:mm')" 2>$null
    git push -u origin main

    Write-Host "✅ Admin dashboard deployed!" -ForegroundColor Green
    Write-Host "   URL: https://manager.avila.inc" -ForegroundColor Cyan
    Write-Host ""
}

# Executar deploys
if ($DeployBoth -or ($DeployLanding -and $DeployAdmin)) {
    Deploy-Landing
    Deploy-Admin
} elseif ($DeployLanding) {
    Deploy-Landing
} elseif ($DeployAdmin) {
    Deploy-Admin
} else {
    Write-Host "📋 Uso:" -ForegroundColor Yellow
    Write-Host "   .\deploy-github-pages.ps1 -DeployLanding    # Apenas landing page" -ForegroundColor White
    Write-Host "   .\deploy-github-pages.ps1 -DeployAdmin      # Apenas admin" -ForegroundColor White
    Write-Host "   .\deploy-github-pages.ps1 -DeployBoth       # Ambos" -ForegroundColor White
    Write-Host ""
    Write-Host "🔧 Ou use o script .bat para interface simples" -ForegroundColor Gray
}

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "    DEPLOY CONCLUÍDO!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📝 Próximos passos:" -ForegroundColor Yellow
Write-Host "1. Configure os domínios no GitHub Pages Settings" -ForegroundColor White
Write-Host "2. Aguarde 5-10 minutos para deploy" -ForegroundColor White
Write-Host "3. Configure DNS no seu provedor de domínio" -ForegroundColor White
Write-Host "4. Aguarde até 24h para propagação DNS" -ForegroundColor White
Write-Host ""
Write-Host "📖 Ver guia completo: GITHUB-PAGES-SETUP.md" -ForegroundColor Cyan