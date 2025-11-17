# 🌐 Configuração de Domínio Personalizado - avila.inc

## Problema Identificado

❌ **avila.inc** - Sem registros DNS (não funciona)
✅ **salmon-island-0f049391e.3.azurestaticapps.net** - Funcionando perfeitamente

## Causa

O domínio `avila.inc` está registrado mas **não tem DNS configurado** apontando para o Azure Static Web App.

## Solução: Configurar DNS

### Opção 1: Usar Registrar do Domínio (Recomendado)

Onde você registrou o domínio `avila.inc`? (GoDaddy, Namecheap, Google Domains, etc)

#### Passo a Passo:

1. **Acesse o painel do seu registrador de domínio**
2. **Vá em DNS Management / Gerenciamento de DNS**
3. **Adicione os seguintes registros:**

#### Para domínio raiz (avila.inc)

**Registro A:**
```
Type: A
Name: @  (ou deixe em branco)
Value: 20.65.18.151
TTL: 3600 (1 hora)
```

**Registro TXT (validação):**
```
Type: TXT
Name: asuid
Value: [código de validação do Azure - veja abaixo como obter]
TTL: 3600
```

#### Para subdomínio www (www.avila.inc)

**Registro CNAME:**
```
Type: CNAME
Name: www
Value: salmon-island-0f049391e.3.azurestaticapps.net
TTL: 3600
```

### Opção 2: Usar Azure DNS (Profissional)

Se quiser usar o Azure DNS (mais integrado):

1. **Criar Zona DNS no Azure:**
```bash
az network dns zone create \
  --resource-group avila-resources \
  --name avila.inc
```

2. **Obter Name Servers do Azure:**
```bash
az network dns zone show \
  --resource-group avila-resources \
  --name avila.inc \
  --query nameServers
```

3. **No registrador do domínio**, altere os Name Servers para os fornecidos pelo Azure

4. **Adicionar registros no Azure DNS:**
```bash
# Registro A para domínio raiz
az network dns record-set a add-record \
  --resource-group avila-resources \
  --zone-name avila.inc \
  --record-set-name @ \
  --ipv4-address 20.65.18.151

# CNAME para www
az network dns record-set cname set-record \
  --resource-group avila-resources \
  --zone-name avila.inc \
  --record-set-name www \
  --cname salmon-island-0f049391e.3.azurestaticapps.net
```

## Configuração no Azure Static Web Apps

### 1. Obter o Código de Validação

```bash
# Via Azure CLI
az staticwebapp show \
  --name salmon-island-0f049391e \
  --resource-group [seu-resource-group] \
  --query "customDomains"
```

Ou pelo **Portal Azure**:
1. Acesse: https://portal.azure.com
2. Vá em **Static Web Apps**
3. Selecione: `salmon-island-0f049391e`
4. Menu lateral: **Custom domains**
5. Clique: **+ Add**
6. Domínio: `avila.inc`
7. Copie o **código de validação TXT**

### 2. Adicionar Domínio Personalizado

**Via Portal:**
1. Em "Custom domains", clique **+ Add**
2. Digite: `avila.inc`
3. Selecione: **Custom domain on other DNS**
4. Clique: **Next**
5. Copie o registro TXT de validação
6. Adicione no seu DNS
7. Aguarde propagação (5-60 minutos)
8. Clique: **Validate**

**Via Azure CLI:**
```bash
az staticwebapp hostname set \
  --name salmon-island-0f049391e \
  --resource-group [seu-resource-group] \
  --hostname avila.inc
```

### 3. Adicionar www.avila.inc também

Repita o processo para `www.avila.inc`:
```bash
az staticwebapp hostname set \
  --name salmon-island-0f049391e \
  --resource-group [seu-resource-group] \
  --hostname www.avila.inc
```

## Configuração SSL/HTTPS

O Azure Static Web Apps **automaticamente provisiona certificado SSL** (Let's Encrypt) após validação do domínio!

⏱️ Tempo: 5-30 minutos após validação DNS

## Redirecionamentos

Após configurar, adicione redirecionamentos no arquivo `staticwebapp.config.json`:

```json
{
  "routes": [
    {
      "route": "/*",
      "allowedRoles": ["anonymous", "authenticated"]
    }
  ],
  "navigationFallback": {
    "rewrite": "/index.html"
  },
  "globalHeaders": {
    "Strict-Transport-Security": "max-age=31536000; includeSubDomains"
  },
  "redirects": [
    {
      "source": "www.avila.inc/*",
      "destination": "https://avila.inc/:splat",
      "statusCode": 301
    }
  ]
}
```

## Registradores Comuns - Guias Específicos

### GoDaddy
1. Login em: https://dcc.godaddy.com/domains
2. Selecione: `avila.inc`
3. Clique: **DNS** > **Manage Zones**
4. Adicione registros A e CNAME

### Namecheap
1. Login em: https://ap.www.namecheap.com/
2. **Domain List** > `avila.inc`
3. **Advanced DNS**
4. Adicione registros

### Google Domains
1. Login em: https://domains.google.com/
2. Selecione: `avila.inc`
3. **DNS**
4. **Manage custom records**

### Registro.br (Brasil)
1. Login em: https://registro.br/
2. Selecione: `avila.inc`
3. **Alterar servidores DNS**
4. Adicione registros

## Verificação

Após configurar DNS, aguarde propagação e teste:

```bash
# Verificar DNS
nslookup avila.inc
nslookup www.avila.inc

# Verificar com dig (mais detalhado)
dig avila.inc
dig www.avila.inc

# Testar HTTPS
curl -I https://avila.inc
```

## Propagação DNS

⏱️ **Tempo de propagação:** 5 minutos a 48 horas (geralmente < 2 horas)

Ferramenta para verificar propagação global:
- https://www.whatsmydns.net/#A/avila.inc
- https://dnschecker.org/

## Checklist

- [ ] Domínio registrado e ativo
- [ ] Registro A criado (@ → 20.65.18.151)
- [ ] Registro CNAME criado (www → salmon-island-*.azurestaticapps.net)
- [ ] Registro TXT de validação adicionado
- [ ] Domínio adicionado no Azure Static Web App
- [ ] Validação DNS confirmada no Azure
- [ ] SSL/HTTPS funcionando
- [ ] Redirecionamento www → raiz configurado
- [ ] Testado em múltiplos navegadores

## Troubleshooting

### DNS não propaga
- Aguarde até 48h
- Verifique no registrador se salvou corretamente
- Use modo anônimo/incógnito para testar
- Limpe cache DNS: `ipconfig /flushdns` (Windows) ou `sudo dscacheutil -flushcache` (Mac)

### "Domain validation failed"
- Verifique registro TXT exato
- Aguarde propagação completa
- TTL muito alto pode demorar mais

### SSL não ativa
- Aguarde até 30 minutos após validação
- Certifique-se que DNS está correto
- Verifique no portal se validação passou

### Erros 404
- Verifique deployment no Azure
- Confirme que arquivos estão na raiz correta
- Veja logs do Static Web App

## Suporte

- **Azure Static Web Apps Docs**: https://docs.microsoft.com/azure/static-web-apps/
- **DNS Propagation Checker**: https://www.whatsmydns.net/
- **SSL Checker**: https://www.ssllabs.com/ssltest/

---

**Ávila Inc** - Guia de Configuração DNS
Última atualização: Novembro 2025
