# Avx Manager - Control Plane

**Control plane interno da Avx** para governar clientes, projetos, templates, repositórios, deploy, DNS, pagamentos e auditoria.

**Arquitetura:** 1 Engine + N Instâncias (instância = projeto/cliente; engine = sistema)

---

## 🏗️ Arquitetura

```
┌─────────────────┐
│  Manager.Web    │  ← Blazor UI (Dashboard)
│   (Blazor)      │
└────────┬────────┘
         │
         │ HTTP
         ▼
┌─────────────────┐     ┌──────────────────┐
│  Manager.Api    │────►│  Manager.Worker  │
│   (REST API)    │     │  (Jobs/Playbooks)│
└────────┬────────┘     └──────────────────┘
         │                        │
         │                        │
         ▼                        ▼
    ┌────────────────────────────────┐
    │        PostgreSQL DB           │
    │   (Projects, Clients, Runs)    │
    └────────────────────────────────┘
              │
              │
         ┌────┴────┐
         ▼         ▼
    ┌────────┐ ┌──────────┐
    │ GitHub │ │Cloudflare│
    └────────┘ └──────────┘
```

---

## 📦 Projetos

### Manager.Core
- **Domínio:** Entidades, Enums, Value Objects
- **DDD:** Identity, Clients, Projects, Provisioning, Deploy, Orchestration, Audit

### Manager.Infrastructure
- **Data:** EF Core DbContext, Migrations
- **Repositories:** Generic + específicos (Project, PlaybookRun)
- **Services:** SecretService (criptografia)

### Manager.Api
- **REST API:** Controllers (Clients, Projects, PlaybookRuns)
- **DTOs:** Request/Response models
- **Auth:** JWT (futuro)
- **Seed:** Database seeder automático

### Manager.Worker
- **Background Service:** Processa jobs assíncronos
- **PlaybookRunner:** Orquestração de playbooks
- **JobProcessor:** Fila de execução

### Manager.Web
- **Blazor Server:** Dashboard interativo
- **Pages:** Projects, Project Detail, Playbook Runs
- **Components:** StatusBadge, JobStatusBadge

### Manager.Integrations
- **GitHub:** CreateRepo, SeedTemplate, ConfigurePages
- **Playbooks:** Landing, Website, System
- **Steps:** Modular execution units

---

## 🚀 Quick Start

### 1. Prerequisites
```bash
# Instalar .NET 10 SDK
# Instalar PostgreSQL

# Verificar instalação
dotnet --version  # 10.0+
psql --version    # 14+
```

### 2. Database Setup
```bash
# Criar database
createdb manager

# Aplicar migrations
dotnet ef database update \
  -p src/Manager.Infrastructure \
  -s src/Manager.Api
```

### 3. Configuration
```bash
# Copiar .env.example para .env
cp .env.example .env

# Editar .env com suas credenciais
nano .env
```

Variáveis obrigatórias:
- `ConnectionStrings__DefaultConnection`
- `Secrets__EncryptionKey` (mínimo 32 caracteres)
- `GitHub__Token` (Personal Access Token)
- `GitHub__Owner` (username ou org)

### 4. Run Services

**Terminal 1 - API:**
```bash
cd src/Manager.Api
dotnet run
# API: https://localhost:7000
```

**Terminal 2 - Worker:**
```bash
cd src/Manager.Worker
dotnet run
# Worker processing jobs in background
```

**Terminal 3 - Web:**
```bash
cd src/Manager.Web
dotnet run
# UI: https://localhost:5001
```

---

## 📚 API Endpoints

### Projects
```http
GET    /api/projects
GET    /api/projects/{id}
POST   /api/projects
PUT    /api/projects/{id}
DELETE /api/projects/{id}
```

### Clients
```http
GET    /api/clients
GET    /api/clients/{id}
POST   /api/clients
```

### Playbook Runs
```http
GET    /api/playbook-runs
GET    /api/playbook-runs?projectId={id}
GET    /api/playbook-runs/{id}
```

---

## 🎯 Playbooks

### Landing (GitHub Pages)
1. **ValidateSpec** - Valida ProjectSpec
2. **CreateRepo** - Cria repositório no GitHub
3. **SeedTemplate** - Popula com template
4. **ConfigurePages** - Ativa GitHub Pages
5. **TriggerBuild** - Dispara workflow
6. **EmitDNSInstructions** - Gera instruções DNS
7. **MarkLive** - Marca projeto como Live

### Website (Multi-page)
1. **ValidateSpec**
2. **CreateRepo**
3. **SeedTemplate** (website-engine)
4. **ConfigurePages**
5. **TriggerBuild**

---

## 📊 Project Status Machine

```
Draft → AwaitingSpec → AwaitingPayment → QueuedProvisioning 
  → ProvisioningRepo → Deploying → AwaitingDNS → Live

                    ↓ Error ↓
                   Suspended
```

Cada transição gera **AuditEvent** automático.

---

## 🔐 Security

### Secrets Management
- **Desenvolvimento:** Criptografia simples (AES)
- **Produção:** Azure Key Vault / AWS Secrets Manager

### Encryption
```csharp
// SecretService criptografa automaticamente
var secret = new Secret {
    Key = "GITHUB_TOKEN",
    EncryptedValue = secretService.Encrypt(token)
};
```

---

## 🧪 Development

### Build All
```bash
dotnet build Manager.sln
```

### Run Tests
```bash
dotnet test
```

### Migrations
```bash
# Criar nova migration
dotnet ef migrations add MigrationName \
  -p src/Manager.Infrastructure \
  -s src/Manager.Api

# Aplicar
dotnet ef database update \
  -p src/Manager.Infrastructure \
  -s src/Manager.Api
```

---

## 📁 Project Structure

```
/src
  /Manager.Core          # Domain (Entities, Enums)
  /Manager.Infrastructure # Data (EF, Repos)
  /Manager.Api           # REST API
  /Manager.Worker        # Background Jobs
  /Manager.Web           # Blazor UI
  /Manager.Integrations  # GitHub, Cloudflare, etc
/tests
  /Manager.Tests         # Unit/Integration tests
/docs
  /CONFIGURATION.md      # Environment setup
  /README.md             # This file
/templates
  /landing-engine        # Landing page template
  /website-engine        # Website template
```

---

## 🎨 Tech Stack

- **.NET 10** - Runtime
- **C# 13** - Language
- **EF Core 10** - ORM
- **PostgreSQL** - Database
- **Blazor Server** - UI
- **Octokit** - GitHub integration
- **Bootstrap 5** - CSS

---

## 🚦 Roadmap

### ✅ Fase 0 + 1 + 2 (Completed)
- [x] Domain entities
- [x] Database + migrations
- [x] REST API (CRUD)
- [x] Blazor UI
- [x] PlaybookRunner
- [x] GitHub integration

### 🔜 Fase 3 - DNS
- [ ] Cloudflare API integration
- [ ] Auto DNS record creation
- [ ] Domain verification

### 🔜 Fase 4 - Billing
- [ ] Asaas/Stripe integration
- [ ] Invoice generation
- [ ] Entitlement gates (Live/Suspended)

---

## 📞 Support

Para dúvidas ou issues, consulte:
- `docs/CONFIGURATION.md` - Setup detalhado
- `docs/ARCHITECTURE.md` - Arquitetura completa
- GitHub Issues - Reportar bugs

---

## 📄 License

Proprietary - Avx © 2025
