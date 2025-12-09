# Script para testar todos os endpoints do servidor Rust

Write-Host "🧪 Testando Endpoints do Servidor Rust Backend" -ForegroundColor Cyan
Write-Host "=" * 60 -ForegroundColor Gray
Write-Host ""

$baseUrl = "http://localhost:3005"

function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Url
    )
    
    Write-Host "📍 Testando: $Name" -ForegroundColor Yellow
    Write-Host "   URL: $Url" -ForegroundColor Gray
    
    try {
        $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 5
        Write-Host "   ✅ Status: $($response.StatusCode)" -ForegroundColor Green
        Write-Host "   📦 Response:" -ForegroundColor Gray
        
        # Pretty print JSON
        $json = $response.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10
        Write-Host $json -ForegroundColor White
        Write-Host ""
    }
    catch {
        Write-Host "   ❌ Erro: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
    }
}

# Aguardar servidor iniciar
Write-Host "⏳ Aguardando servidor iniciar..." -ForegroundColor Yellow
Start-Sleep -Seconds 3
Write-Host ""

# Testar endpoints
Test-Endpoint -Name "Root Endpoint" -Url "$baseUrl/"
Test-Endpoint -Name "Health Check" -Url "$baseUrl/health"
Test-Endpoint -Name "GitHub Repository Info" -Url "$baseUrl/api/github/repository"
Test-Endpoint -Name "GitHub Issues" -Url "$baseUrl/api/github/issues"
Test-Endpoint -Name "GitHub Pull Requests" -Url "$baseUrl/api/github/pulls"
Test-Endpoint -Name "GitHub Branches" -Url "$baseUrl/api/github/branches"
Test-Endpoint -Name "GitHub Commits" -Url "$baseUrl/api/github/commits"

Write-Host "=" * 60 -ForegroundColor Gray
Write-Host "✅ Testes concluídos!" -ForegroundColor Green
