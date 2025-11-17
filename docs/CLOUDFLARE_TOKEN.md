# 🔑 Gerar Novo Token Cloudflare API

## O token atual está inválido/expirado

Para continuar, você precisa gerar um novo token:

## Passo a Passo:

### 1. Acesse o Cloudflare Dashboard
https://dash.cloudflare.com/

### 2. Vá em API Tokens
- Perfil (canto superior direito) → **My Profile**
- Menu lateral → **API Tokens**
- Ou direto: https://dash.cloudflare.com/profile/api-tokens

### 3. Crie um Novo Token
Clique em: **Create Token**

### 4. Use o Template "Edit zone DNS"
- Procure: **Edit zone DNS**
- Clique: **Use template**

### 5. Configure as Permissões

**Permissions (Permissões):**
```
Zone - DNS - Edit
Zone - Zone - Read
```

**Zone Resources (Recursos de Zona):**
```
Include - Specific zone - avila.inc
```

Ou se preferir acesso a todos os domínios:
```
Include - All zones
```

### 6. Gere o Token
- Clique: **Continue to summary**
- Revise as permissões
- Clique: **Create Token**

### 7. Copie o Token
⚠️ **IMPORTANTE:** O token aparece **apenas uma vez**!

Exemplo:
```
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
```

### 8. Atualize o .env

Abra o arquivo `.env` e atualize:

```env
CLOUDFLARE_API_TOKEN=SEU_NOVO_TOKEN_AQUI
CLOUDFLARE_ACCOUNT_ID=85b914dcdebf6fa2fce7b819bc542294
```

### 9. Teste o Novo Token

```bash
# Via Wrangler
$env:CLOUDFLARE_API_TOKEN="SEU_NOVO_TOKEN"; wrangler whoami

# Via script Node.js
node scripts/cloudflare-dns.js

# Via curl
curl "https://api.cloudflare.com/client/v4/user/tokens/verify" `
  -H "Authorization: Bearer SEU_NOVO_TOKEN" `
  -H "Content-Type: application/json"
```

## Alternativa Rápida: Usar a Interface Web

Se não quiser usar a API, você pode configurar tudo pela interface:

### 1. Dashboard Cloudflare
https://dash.cloudflare.com/

### 2. Selecione avila.inc

### 3. Vá em DNS → Records

### 4. Configure os registros:

**Deletar (se existir):**
- CNAME `avila.inc` → salmon-island...

**Adicionar:**
- Type: **A**
- Name: **@** (ou avila.inc)
- IPv4: **20.65.18.151**
- Proxy: **🔘 DNS only** (desativado/cinza)
- Clique: **Save**

**Verificar:**
- CNAME `www` → salmon-island-0f049391e.3.azurestaticapps.net
- Proxy: **🔘 DNS only** (desativado/cinza)

**Aguardar validação Azure:**
- Após adicionar o domínio no Azure
- Copie o TXT que o Azure fornecer
- Adicione aqui: Type=TXT, Name=asuid, Value=[código do Azure]

## Próximo Passo Após Token Válido

Execute novamente:
```bash
node scripts/cloudflare-dns.js
```

Isso vai:
- ✅ Verificar token
- ✅ Listar domínios
- ✅ Mostrar registros DNS atuais
- ✅ Analisar configuração
- ✅ Sugerir correções

---

**Precisa de ajuda?** Consulte: https://developers.cloudflare.com/fundamentals/api/get-started/create-token/
