#!/usr/bin/env pwsh
# Script para testar e configurar Cloudflare API

# Carregar variáveis do .env
$envFile = Join-Path $PSScriptRoot ".." ".env"
if (Test-Path $envFile) {
    Get-Content $envFile | ForEach-Object {
        if ($_ -match '^([^#][^=]+)=(.*)$') {
            $name = $matches[1].Trim()
            $value = $matches[2].Trim()
            [Environment]::SetEnvironmentVariable($name, $value, "Process")
        }
    }
    Write-Host "✅ Variáveis carregadas de .env" -ForegroundColor Green
}

$apiToken = $env:CLOUDFLARE_API_TOKEN
$accountId = $env:CLOUDFLARE_ACCOUNT_ID

if (-not $apiToken) {
    Write-Host "❌ CLOUDFLARE_API_TOKEN não encontrada no .env" -ForegroundColor Red
    exit 1
}

Write-Host "`n🌐 Testando Cloudflare API..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan

# 1. Verificar Token
Write-Host "`n📋 1. Verificando Token..." -ForegroundColor Yellow
$verifyUrl = "https://api.cloudflare.com/client/v4/user/tokens/verify"
$headers = @{
    "Authorization" = "Bearer $apiToken"
    "Content-Type" = "application/json"
}

try {
    $response = Invoke-RestMethod -Uri $verifyUrl -Headers $headers -Method Get
    if ($response.success) {
        Write-Host "✅ Token válido!" -ForegroundColor Green
        Write-Host "   Status: $($response.result.status)" -ForegroundColor Gray
        Write-Host "   ID: $($response.result.id)" -ForegroundColor Gray
    } else {
        Write-Host "❌ Token inválido!" -ForegroundColor Red
        Write-Host "   Erros: $($response.errors | ConvertTo-Json)" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "❌ Erro ao verificar token: $_" -ForegroundColor Red
    exit 1
}

# 2. Listar Zonas (Domínios)
Write-Host "`n📋 2. Listando domínios/zonas..." -ForegroundColor Yellow
$zonesUrl = "https://api.cloudflare.com/client/v4/zones"

try {
    $zonesResponse = Invoke-RestMethod -Uri $zonesUrl -Headers $headers -Method Get

    if ($zonesResponse.success) {
        Write-Host "✅ Domínios encontrados:" -ForegroundColor Green

        $avilaZone = $null
        foreach ($zone in $zonesResponse.result) {
            $status = if ($zone.status -eq "active") { "🟢" } else { "🟡" }
            Write-Host "   $status $($zone.name) (ID: $($zone.id))" -ForegroundColor Cyan

            if ($zone.name -eq "avila.inc") {
                $avilaZone = $zone
                Write-Host "      👆 Domínio principal encontrado!" -ForegroundColor Green
            }
        }

        if ($avilaZone) {
            $zoneId = $avilaZone.id
            Write-Host "`n💡 Adicione ao .env:" -ForegroundColor Yellow
            Write-Host "   CLOUDFLARE_ZONE_ID=$zoneId" -ForegroundColor White

            # Atualizar automaticamente o .env
            $envContent = Get-Content $envFile
            $envContent = $envContent -replace 'CLOUDFLARE_ZONE_ID=.*', "CLOUDFLARE_ZONE_ID=$zoneId"
            $envContent | Set-Content $envFile
            Write-Host "✅ .env atualizado automaticamente!" -ForegroundColor Green
        }
    } else {
        Write-Host "❌ Erro ao listar zonas" -ForegroundColor Red
    }
} catch {
    Write-Host "⚠️ Erro ao listar zonas: $_" -ForegroundColor Yellow
}

# 3. Listar Registros DNS do avila.inc
if ($avilaZone) {
    Write-Host "`n📋 3. Registros DNS de avila.inc..." -ForegroundColor Yellow
    $dnsUrl = "https://api.cloudflare.com/client/v4/zones/$($avilaZone.id)/dns_records"

    try {
        $dnsResponse = Invoke-RestMethod -Uri $dnsUrl -Headers $headers -Method Get

        if ($dnsResponse.success) {
            Write-Host "✅ Registros DNS:" -ForegroundColor Green

            foreach ($record in $dnsResponse.result) {
                $proxied = if ($record.proxied) { "🟠 Proxied" } else { "🔘 DNS only" }
                Write-Host "   $($record.type.PadRight(6)) $($record.name.PadRight(30)) → $($record.content.PadRight(50)) $proxied" -ForegroundColor Cyan
            }

            # Verificar se tem registro A para raiz
            $rootA = $dnsResponse.result | Where-Object { $_.type -eq "A" -and $_.name -eq "avila.inc" }
            $wwwCNAME = $dnsResponse.result | Where-Object { $_.type -eq "CNAME" -and $_.name -eq "www.avila.inc" }
            $txtAsuid = $dnsResponse.result | Where-Object { $_.type -eq "TXT" -and $_.name -match "asuid" }

            Write-Host "`n📊 Status de Configuração:" -ForegroundColor Yellow

            if ($rootA -and $rootA.content -eq "20.65.18.151") {
                Write-Host "   ✅ Registro A (avila.inc → 20.65.18.151)" -ForegroundColor Green
            } else {
                Write-Host "   ❌ Registro A não configurado corretamente" -ForegroundColor Red
                Write-Host "      Deve ser: A @ → 20.65.18.151" -ForegroundColor Yellow
            }

            if ($wwwCNAME -and $wwwCNAME.content -match "azurestaticapps") {
                Write-Host "   ✅ Registro CNAME (www → Azure)" -ForegroundColor Green
            } else {
                Write-Host "   ⚠️ Registro CNAME www não encontrado" -ForegroundColor Yellow
            }

            if ($txtAsuid) {
                Write-Host "   ✅ Registro TXT de validação Azure encontrado" -ForegroundColor Green
            } else {
                Write-Host "   ⚠️ Registro TXT asuid não encontrado (necessário para validação Azure)" -ForegroundColor Yellow
            }
        }
    } catch {
        Write-Host "⚠️ Erro ao listar DNS: $_" -ForegroundColor Yellow
    }
}

# 4. Testar resolução DNS
Write-Host "`n📋 4. Testando resolução DNS..." -ForegroundColor Yellow

try {
    $dnsTest = Resolve-DnsName -Name "avila.inc" -Type A -ErrorAction SilentlyContinue
    if ($dnsTest -and $dnsTest.IPAddress -eq "20.65.18.151") {
        Write-Host "   ✅ avila.inc resolve para 20.65.18.151" -ForegroundColor Green
    } elseif ($dnsTest) {
        Write-Host "   ⚠️ avila.inc resolve para: $($dnsTest.IPAddress)" -ForegroundColor Yellow
    } else {
        Write-Host "   ❌ avila.inc não resolve" -ForegroundColor Red
    }
} catch {
    Write-Host "   ⚠️ Erro ao resolver DNS: $_" -ForegroundColor Yellow
}

# 5. Testar conectividade HTTP
Write-Host "`n📋 5. Testando conectividade HTTP..." -ForegroundColor Yellow

try {
    $httpTest = Invoke-WebRequest -Uri "http://avila.inc" -MaximumRedirection 0 -ErrorAction Stop
    Write-Host "   ✅ HTTP responde (Status: $($httpTest.StatusCode))" -ForegroundColor Green
} catch {
    if ($_.Exception.Response.StatusCode.Value__ -eq 301 -or $_.Exception.Response.StatusCode.Value__ -eq 302) {
        Write-Host "   ✅ HTTP redireciona (esperado)" -ForegroundColor Green
    } else {
        Write-Host "   ❌ HTTP não responde: $_" -ForegroundColor Red
    }
}

try {
    $httpsTest = Invoke-WebRequest -Uri "https://avila.inc" -ErrorAction Stop
    Write-Host "   ✅ HTTPS funciona! (Status: $($httpsTest.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "   ⚠️ HTTPS ainda não configurado (normal se acabou de configurar)" -ForegroundColor Yellow
    Write-Host "      Aguarde 5-30 minutos para SSL ser provisionado" -ForegroundColor Gray
}

# Resumo final
Write-Host "`n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "📊 RESUMO" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan

Write-Host "`n✅ API Token: Válida" -ForegroundColor Green

if ($avilaZone) {
    Write-Host "✅ Domínio avila.inc: Encontrado" -ForegroundColor Green
} else {
    Write-Host "❌ Domínio avila.inc: Não encontrado" -ForegroundColor Red
}

Write-Host "`n📝 Próximos Passos:" -ForegroundColor Yellow
Write-Host "   1. Verifique os registros DNS acima" -ForegroundColor White
Write-Host "   2. Vá no Azure Portal: Custom domains" -ForegroundColor White
Write-Host "   3. Adicione avila.inc e valide" -ForegroundColor White
Write-Host "   4. Aguarde 5-30min para SSL" -ForegroundColor White
Write-Host "`n🌐 Azure Portal: https://portal.azure.com" -ForegroundColor Cyan
Write-Host "🌐 Cloudflare: https://dash.cloudflare.com" -ForegroundColor Cyan

Write-Host "`n✨ Script concluído!`n" -ForegroundColor Green
