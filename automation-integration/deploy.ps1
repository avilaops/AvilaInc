# Avila Automation Integration - Deploy Script
# PowerShell version

Write-Host "🚀 Avila Automation Integration - Deploy Script" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Build backend Rust
Write-Host "📦 Building Rust backend..." -ForegroundColor Yellow
Push-Location backend-rs
cargo build --release
Pop-Location

Write-Host "✅ Build completed!" -ForegroundColor Green
Write-Host ""

Write-Host "📊 Binary size:" -ForegroundColor Cyan
Get-Item backend-rs\target\release\automation-integration.exe | Select-Object Name, @{Name="Size (MB)";Expression={[math]::Round($_.Length/1MB,2)}}

Write-Host ""
Write-Host "🎯 To run the server:" -ForegroundColor Yellow
Write-Host '  $env:PORT=3005; .\backend-rs\target\release\automation-integration.exe' -ForegroundColor White
Write-Host ""
Write-Host "Or with GitHub token:" -ForegroundColor Yellow
Write-Host '  $env:PORT=3005; $env:GITHUB_TOKEN="your_token"; .\backend-rs\target\release\automation-integration.exe' -ForegroundColor White
