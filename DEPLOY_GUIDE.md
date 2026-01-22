# 🚀 Guia de Deploy - Avila Manager

## 📋 Estratégia de Deploy

Este projeto segue uma **estratégia profissional de deploy** com ambientes separados e CI/CD automatizado.

### ✅ O que é BOM fazer:
- **Deploy frequente** (várias vezes por dia)
- **Deploy pequeno** (uma feature por vez)
- **Deploy automatizado** (GitHub Actions)
- **Ambientes separados** (staging + produção)

### ❌ O que NÃO fazer:
- Deploy manual direto em produção
- Deploy sem teste em staging
- Deploy "pra ver se funciona"
- Deploy sem versionamento

---

## 🌿 Branches e Ambientes

### 📂 Estrutura de Branches

```
main (produção)     → https://landing-web.azurewebsites.net
  ↑
develop (staging)   → https://landing-web-staging.azurewebsites.net
  ↑
feature/*           → desenvolvimento local
```

### 🎯 Quando usar cada branch:

| Branch | Ambiente | Quando usar | Deploy automático |
|--------|----------|-------------|-------------------|
| `main` | **Produção** | Código testado e aprovado | ✅ Sim |
| `develop` | **Staging** | Testes e validações | ✅ Sim |
| `feature/*` | **Local** | Desenvolvimento | ❌ Não |

---

## 🔄 Fluxo de Desenvolvimento

### 1️⃣ Desenvolvimento Local
```bash
# Criar feature branch
git checkout -b feature/nova-funcionalidade

# Desenvolver e testar localmente
# ... código ...

# Commit das mudanças
git add .
git commit -m "feat: adicionar nova funcionalidade"
```

### 2️⃣ Push para Staging
```bash
# Push para develop (staging)
git checkout develop
git merge feature/nova-funcionalidade
git push origin develop
```
→ **Deploy automático para staging** 🚀

### 3️⃣ Teste em Staging
- ✅ Testar funcionalidade
- ✅ Verificar se não quebrou nada
- ✅ Validar com usuário/equipe

### 4️⃣ Deploy para Produção
```bash
# Merge para main (produção)
git checkout main
git merge develop
git push origin main
```
→ **Deploy automático para produção** 🎉

---

## ⚙️ Configuração dos Workflows

### 📁 Workflows GitHub Actions

| Workflow | Trigger | Ambiente | App |
|----------|---------|----------|-----|
| `staging_landing-web.yml` | Push `develop` | Staging | Landing Page |
| `staging_manager-api.yml` | Push `develop` | Staging | Manager API |
| `main_landing-web.yml` | Push `main` | Produção | Landing Page |
| `main_manager-api.yml` | Push `main` | Produção | Manager API |

### 🔐 Secrets Necessários

No GitHub → Settings → Secrets and variables → Actions:

#### Azure Authentication (OIDC - opcional)
- `AZUREAPPSERVICE_CLIENTID_*`
- `AZUREAPPSERVICE_TENANTID_*`
- `AZUREAPPSERVICE_SUBSCRIPTIONID_*`

#### Publish Profiles (recomendado)
- `PUBLISH_PROFILE_API` (Manager API - staging e produção)
- `PUBLISH_PROFILE` (Landing Page - staging e produção)

### 📥 Como obter Publish Profiles

1. **Acesse o Azure Portal**
2. **Navegue para seu App Service** (ex: landing-web)
3. **Clique em "Get publish profile"**
4. **Salve o conteúdo XML no GitHub Secret**

> 💡 **Dica**: Use o mesmo publish profile para staging e produção, pois o slot-name é especificado no workflow.

---

## 🌐 URLs dos Ambientes

### 🏭 Staging (Testes)
- **Landing Page**: https://landing-web-staging.azurewebsites.net
- **Manager API**: https://manager-api-staging.azurewebsites.net

### 🎯 Produção (Clientes)
- **Landing Page**: https://landing-web.azurewebsites.net
- **Manager API**: https://manager-api.azurewebsites.net

---

## 📊 Monitoramento de Deploys

### ✅ Status dos Workflows
- Acesse: GitHub → Actions
- Veja status de cada deploy
- Logs detalhados disponíveis

### 🚨 Em caso de problema:
1. **Reverter**: `git revert` do commit problemático
2. **Rollback**: Azure permite rollback via portal
3. **Hotfix**: Criar branch `hotfix/*` se necessário

---

## 🎯 Boas Práticas

### ✅ Deploy Pequeno
- Uma feature por deploy
- Máximo 1-2 arquivos modificados
- Fácil de reverter se der problema

### ✅ Teste Antes
- Sempre testar em staging primeiro
- Validar com usuários reais
- Não confiar apenas em testes automatizados

### ✅ Comunicação
- Avisar equipe sobre deploys importantes
- Documentar mudanças significativas
- Manter changelog atualizado

### ✅ Segurança
- Nunca commitar secrets
- Usar sempre HTTPS
- Manter dependências atualizadas

---

## 🚀 Deploy Manual (Excepcional)

**Apenas em casos extremos** (não é recomendado):

```bash
# Via GitHub Actions (recomendado)
gh workflow run "Deploy Landing to Production" --ref main

# Ou via Azure CLI
az webapp deployment source config --name landing-web --resource-group your-rg --repo-url https://github.com/your-org/admin --branch main
```

---

## 📞 Suporte

**Problemas com deploy?**
1. Verificar logs no GitHub Actions
2. Checar status no Azure Portal
3. Validar configurações de secrets
4. Abrir issue no repositório

---

**🎉 Deploy frequente = Inovação constante!**