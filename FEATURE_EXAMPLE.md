# 🚀 Exemplo: Criando uma Nova Feature

Este exemplo mostra como implementar uma nova funcionalidade seguindo o fluxo profissional de deploy.

## 🎯 Cenário: Adicionar Campo "Empresa" no Formulário de Lead

### 1️⃣ Criar Feature Branch

```bash
# Criar branch da feature
git checkout develop
git pull origin develop
git checkout -b feature/lead-company-field

# Verificar branch
git branch
# * feature/lead-company-field
#   develop
#   main
```

### 2️⃣ Implementar a Funcionalidade

#### 📝 Backend - Atualizar DTO
```csharp
// src/Manager.Contracts/DTOs/LeadDTOs.cs
public sealed record CreateLeadDto(
    string Name,
    string Email,
    string Phone,
    string? Message,
    string Source,
    string? Company,  // ← Novo campo
    // ... outros campos
);
```

#### 🎨 Frontend - Atualizar Formulário
```razor
<!-- src/Landing/Components/LeadForm.razor -->
<div class="form-group">
    <label for="company">Empresa (Opcional)</label>
    <InputText id="company" @bind-Value="_model.Company"
               placeholder="Nome da empresa"
               disabled="@_isSubmitting" />
</div>
```

#### 🗄️ Database - Atualizar Entidade
```csharp
// src/Manager.Core/Entities/Lead.cs
[BsonElement("company")]
public string? Company { get; set; }
```

### 3️⃣ Testar Localmente

```bash
# Build e teste local
cd src/Landing
dotnet run --urls=http://localhost:3000

cd ../Manager.Api
dotnet run --urls=http://localhost:5056

# Testar formulário e API
```

### 4️⃣ Commit e Push

```bash
# Commit das mudanças
git add .
git commit -m "feat: adicionar campo empresa no formulário de lead

- Adicionar campo Company no DTO CreateLeadDto
- Atualizar formulário LeadForm.razor
- Mapear campo Company na entidade Lead
- Testes locais realizados"

# Push para branch da feature
git push origin feature/lead-company-field
```

### 5️⃣ Criar Pull Request

1. Ir para GitHub → Pull Requests
2. "New Pull Request"
3. Base: `develop` ← Compare: `feature/lead-company-field`
4. Título: "feat: adicionar campo empresa no formulário de lead"
5. Descrição: Explicar mudanças e testes realizados

### 6️⃣ Merge para Staging

Após aprovação do PR:
```bash
# Merge via GitHub ou:
git checkout develop
git merge feature/lead-company-field
git push origin develop
```

→ **Deploy automático para staging** 🚀

### 7️⃣ Teste em Staging

- ✅ Acessar: https://landing-web-staging.azurewebsites.net
- ✅ Testar formulário com campo empresa
- ✅ Verificar dados no admin staging
- ✅ Validar com equipe/usuários

### 8️⃣ Deploy para Produção

```bash
# Merge para produção
git checkout main
git merge develop
git push origin main
```

→ **Deploy automático para produção** 🎉

---

## 📊 Resultado

| Ambiente | URL | Status |
|----------|-----|--------|
| **Staging** | https://landing-web-staging.azurewebsites.net | ✅ Campo empresa funcionando |
| **Produção** | https://landing-web.azurewebsites.net | ✅ Campo empresa disponível |

## 🎯 Benefícios Desta Abordagem

✅ **Deploy pequeno**: Uma feature por vez
✅ **Reversível**: Fácil rollback se necessário
✅ **Testado**: Ambiente staging antes da produção
✅ **Versionado**: Git history completo
✅ **Colaborativo**: Code review obrigatório
✅ **Automatizado**: CI/CD cuida do deploy

---

## 🛠️ Troubleshooting

### ❌ Build falhando?
```bash
# Verificar erros
dotnet build

# Limpar cache
dotnet clean
dotnet restore
```

### ❌ Deploy não iniciou?
- Verificar se push foi para branch correta
- Checar GitHub Actions tab
- Validar secrets configurados

### ❌ Funcionalidade não funciona?
- Testar em staging primeiro
- Verificar logs da aplicação
- Validar configuração do ambiente

---

**💡 Dica**: Sempre teste em staging antes de ir para produção!