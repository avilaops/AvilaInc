# 🌐 Guia Completo - Cloudflare + Azure Static Web Apps

## Passo 1: Configurar DNS no Cloudflare

### 1.1 Acesse o Cloudflare
- Login: https://dash.cloudflare.com/
- Selecione: **avila.inc**
- Menu lateral: **DNS** → **Records**

### 1.2 Adicione os Registros DNS

**❗ IMPORTANTE: Desative o Proxy (clique no ícone 🟠 laranja até ficar cinza)**

#### Registro A (domínio raiz)
```
Type: A
Name: @
IPv4 address: 20.65.18.151
Proxy status: 🔘 DNS only (CINZA - proxy desativado)
TTL: Auto
```
**Clique em "Add Record"**

#### Registro CNAME (www)
```
Type: CNAME
Name: www
Target: salmon-island-0f049391e.3.azurestaticapps.net
Proxy status: 🔘 DNS only (CINZA - proxy desativado)
TTL: Auto
```
**Clique em "Add Record"**

### 1.3 Aguarde 2 minutos

Cloudflare propaga muito rápido!

---

## Passo 2: Configurar Domínio Personalizado no Azure

### 2.1 Acesse o Portal Azure
1. Vá em: https://portal.azure.com/
2. No menu lateral ou busca, procure: **Static Web Apps**
3. Clique em: **salmon-island-0f049391e**

### 2.2 Adicionar Domínio
1. Menu lateral: **Custom domains**
2. Clique: **+ Add**
3. Selecione: **Custom domain on other DNS**
4. Digite: `avila.inc`
5. Clique: **Next**

### 2.3 Copiar Código de Validação
Você verá algo assim:
```
Add the following TXT record to your DNS:

Type: TXT
Name: asuid.avila.inc (ou apenas asuid)
Value: 1A2B3C4D5E6F... (código único)
```

### 2.4 Adicionar TXT no Cloudflare
1. Volte no Cloudflare → DNS Records
2. Adicione:
```
Type: TXT
Name: asuid (ou asuid.avila.inc)
Content: [cole o código que copiou do Azure]
Proxy status: 🔘 DNS only
TTL: Auto
```
**Clique em "Add Record"**

### 2.5 Validar no Azure
1. Volte no Portal Azure
2. Aguarde 1-2 minutos
3. Clique: **Validate**
4. Se passar: Clique **Add**

---

## Passo 3: Repetir para www.avila.inc

1. No Azure, clique **+ Add** novamente
2. Digite: `www.avila.inc`
3. Repita o processo de validação
4. Adicione novo TXT (asuid.www.avila.inc) no Cloudflare

---

## Passo 4: Aguardar SSL (Automático)

⏱️ **5-30 minutos** após validação

O Azure provisiona certificado SSL Let's Encrypt automaticamente!

---

## Verificação Rápida

### Teste DNS (PowerShell):
```powershell
# Testar domínio raiz
nslookup avila.inc

# Testar www
nslookup www.avila.inc

# Deve retornar: 20.65.18.151
```

### Testar no Navegador:
```
http://avila.inc (deve funcionar após validação)
https://avila.inc (deve funcionar após SSL)
```

---

## ⚡ Cloudflare - Configurações Adicionais Recomendadas

### SSL/TLS (Após funcionar)
1. **SSL/TLS** → **Overview**
2. Modo: **Full** (não usar Full Strict ainda)

### Regras de Página
1. **Rules** → **Page Rules**
2. URL: `http://avila.inc/*`
3. Setting: **Always Use HTTPS**
4. URL: `http://www.avila.inc/*`
5. Setting: **Forwarding URL** → `https://avila.inc/$1` (301)

### Segurança
1. **Security** → **Settings**
2. Security Level: **Medium**
3. Browser Integrity Check: **On**

### Performance
1. **Speed** → **Optimization**
2. Auto Minify: ☑️ HTML ☑️ CSS ☑️ JavaScript
3. Brotli: **On**

### Firewall (Opcional)
1. **Security** → **WAF**
2. Ative regras managed para proteção extra

---

## 🔧 Troubleshooting

### "DNS not found"
✅ **Solução:** Aguarde 2-5 minutos, Cloudflare é rápido

### "Validation failed" no Azure
✅ **Solução:**
- Certifique que o TXT está correto (copie/cole com cuidado)
- Aguarde 5 minutos
- Tente validar novamente

### "Too many redirects"
✅ **Solução:**
- No Cloudflare: SSL/TLS deve estar em **Full**
- Proxy deve estar **DESATIVADO** (cinza) nos registros DNS

### Site não carrega com HTTPS
✅ **Solução:**
- Aguarde até 30 minutos para SSL provisionar
- Verifique no Azure: Custom domains → deve mostrar 🟢 com cadeado

### Cloudflare Proxy 🟠 vs 🔘
- **🟠 Proxied (laranja):** Tráfego passa pelo Cloudflare (pode causar problemas com SSL do Azure)
- **🔘 DNS Only (cinza):** DNS direto para Azure (RECOMENDADO para Static Web Apps)

---

## ✅ Checklist Final

- [ ] Registro A adicionado no Cloudflare (proxy desativado)
- [ ] Registro CNAME www adicionado no Cloudflare (proxy desativado)
- [ ] Domínio avila.inc adicionado no Azure
- [ ] Registro TXT de validação adicionado no Cloudflare
- [ ] Validação DNS confirmada no Azure
- [ ] Domínio www.avila.inc também adicionado (opcional mas recomendado)
- [ ] SSL funcionando (https://avila.inc)
- [ ] Redirecionamento www → raiz configurado no Cloudflare
- [ ] Site acessível em https://avila.inc

---

## 📱 Comandos Úteis para Testar

### Windows PowerShell
```powershell
# Limpar cache DNS local
ipconfig /flushdns

# Testar DNS
nslookup avila.inc
nslookup www.avila.inc

# Testar conexão HTTP
curl.exe -I http://avila.inc
curl.exe -I https://avila.inc
```

### Ferramentas Online
- **DNS Propagation:** https://www.whatsmydns.net/#A/avila.inc
- **SSL Checker:** https://www.ssllabs.com/ssltest/analyze.html?d=avila.inc
- **Cloudflare DNS Check:** https://1.1.1.1/dns-query?name=avila.inc

---

## 🎯 Resumo Rápido

1. **Cloudflare:** Adicione A e CNAME (proxy DESATIVADO)
2. **Azure Portal:** Custom domains → Add → avila.inc
3. **Cloudflare:** Adicione TXT de validação
4. **Azure Portal:** Valide e adicione
5. **Aguarde:** 5-30 min para SSL
6. **Teste:** https://avila.inc

---

## 💡 Dica Pro

Depois que tudo funcionar, você pode:
- Reativar o proxy 🟠 laranja do Cloudflare para CDN global
- Mas primeiro: SSL/TLS mode deve estar em **Full (strict)**
- E adicione certificado Origin no Azure

Mas por enquanto, deixe **DNS only** (cinza) até funcionar perfeitamente!

---

**Ávila Inc** - Cloudflare Configuration Guide
Última atualização: Novembro 2025
