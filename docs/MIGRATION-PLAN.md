# 🔄 PLANO DE MIGRAÇÃO: Node.js → .NET

## 📋 Objetivo

Migrar **todo** o projeto Admin de Node.js/TypeScript para .NET 10, aproveitando a estrutura Clean Architecture já existente nos projetos Manager.*.

---

## 🔍 Análise Atual

### ✅ **O que já existe em .NET**

#### **Projetos (.NET 10)**
1. **Manager.Api** - REST API com ASP.NET Core
2. **Manager.Core** - Entidades e lógica de domínio
3. **Manager.Infrastructure** - Data access (EF Core)
4. **Manager.Contracts** - DTOs e contratos
5. **Manager.Integrations** - Serviços externos (GitHub, Google, CNPJ)
6. **Manager.Web** - Frontend (Blazor?)
7. **Manager.Worker** - Background jobs

#### **Controllers já implementados**
- ✅ `AuthController` - Login, registro, JWT
- ✅ `CompaniesController` - CRUD de empresas
- ✅ `ClientsController` - Gestão de clientes
- ✅ `ProjectsController` - Projetos
- ✅ `GooglePlacesController` - Google Places API
- ✅ `CnpjController` - Consulta CNPJ
- ✅ `PlaybookRunsController` - Automações

#### **Integrações já migradas**
- ✅ GitHub API (Octokit)
- ✅ Google Places API
- ✅ CNPJ Lookup

#### **Database atual**
- SQLite (desenvolvimento)
- PostgreSQL (produção)
- **EntityFramework Core 10.0**

---

## 🎯 O que precisa ser migrado

### **1. 100+ Endpoints REST do server.js**

#### **Autenticação (7 endpoints)**
- POST `/api/auth/login` ✅ **JÁ EXISTE**
- GET `/api/auth/me` ✅ **JÁ EXISTE**
- POST `/api/auth/refresh`
- POST `/api/auth/forgot-password` ✅ **JÁ EXISTE**
- POST `/api/auth/reset-password` ✅ **JÁ EXISTE**
- PUT `/api/auth/change-password` ✅ **JÁ EXISTE**
- POST `/api/auth/verify-email` ✅ **JÁ EXISTE**

#### **MongoDB Explorer (2 endpoints)** ⚠️ **MIGRAR PARA MONGO**
- GET `/api/mongodb/databases`
- GET `/api/mongodb/collection/:db/:collection`

#### **GitHub (7 endpoints)** 🔄 **PARCIAL**
- GET `/api/github/repos` 🔄 **Service existe, criar Controller**
- GET `/api/github/stats`
- GET `/api/github/issues`
- GET `/api/github/gists`
- GET `/api/github/notifications`
- GET `/api/github/activity`

#### **LinkedIn (4 endpoints)** ❌ **CRIAR**
- GET `/api/linkedin/profile`
- GET `/api/linkedin/posts`
- GET `/api/linkedin/connections`
- GET `/api/linkedin/stats`

#### **Meta/Social (9 endpoints)** ❌ **CRIAR**
- GET `/api/meta/instagram/insights`
- GET `/api/meta/instagram/media`
- GET `/api/meta/instagram/profile`
- GET `/api/meta/facebook/insights`
- GET `/api/meta/facebook/posts`
- GET `/api/meta/whatsapp/info`
- GET `/api/meta/stats`
- POST `/api/social/post`

#### **Stripe (1 endpoint)** ❌ **CRIAR**
- GET `/api/stripe/balance`

#### **OpenAI (5 endpoints)** ❌ **CRIAR**
- GET `/openai/status`
- GET `/openai/models`
- GET `/openai/usage`
- POST `/openai/chat`
- POST `/openai/images/generate`

#### **Railway (1 endpoint)** ❌ **CRIAR**
- GET `/api/railway/projects`

#### **Azure DevOps (1 endpoint)** ❌ **CRIAR**
- GET `/api/azure/organizations`

#### **Google Cloud (1 endpoint)** ❌ **CRIAR**
- GET `/api/gcloud/projects`

#### **Sentry (1 endpoint)** ❌ **CRIAR**
- GET `/api/sentry/issues`

#### **DNS (1 endpoint)** ❌ **CRIAR**
- GET `/api/dns/domains`

#### **Email (1 endpoint)** ❌ **CRIAR**
- POST `/api/email/send`

#### **Calendar (3 endpoints)** ❌ **CRIAR**
- POST `/api/calendar/schedule`
- POST `/api/calendar/save`
- GET `/api/calendar/load`

#### **CRM/Leads (7 endpoints)** 🔄 **PARCIAL**
- POST `/api/crm/leads/webhook`
- POST `/api/crm/leads/patent/verify`
- POST `/api/crm/leads/:id/patent/skip`
- GET `/api/crm/leads`
- PUT `/api/crm/leads/:id/status`
- POST `/api/crm/leads/:id/provision`
- DELETE `/api/crm/leads/:id`
- POST `/api/crm/cliente`
- GET `/api/crm/send-marketing-emails`

#### **Backup/Export (3 endpoints)** ❌ **CRIAR**
- POST `/api/backup/completo`
- GET `/api/export/contatos/csv`
- GET `/api/export/emails/json`

#### **Health Data (1 endpoint)** ❌ **CRIAR**
- GET `/api/health/data`

#### **Financeiro (2 endpoints)** ❌ **CRIAR**
- GET `/api/financeiro/extratos`
- POST `/api/financeiro/atualizar`

#### **E-Reader/Biblioteca (4 endpoints)** ❌ **CRIAR**
- POST `/api/ereader/salvar`
- GET `/api/ereader/carregar`
- GET `/api/ereader/exportar-diario`
- GET `/api/ereader/estatisticas`

#### **Gmail (2 endpoints)** ❌ **CRIAR**
- GET `/api/gmail/stats`

#### **Contacts (3 endpoints)** ❌ **CRIAR**
- GET `/api/contacts/unified`
- GET `/api/contacts/stats`
- GET `/api/contacts/export/csv`

#### **Campanhas (4 endpoints)** ❌ **CRIAR**
- GET `/api/campanhas`
- POST `/api/campanhas`
- PUT `/api/campanhas/:id`
- DELETE `/api/campanhas/:id`

#### **Client Finance (2 endpoints)** ❌ **CRIAR**
- POST `/api/client-finance`
- GET `/api/client-finance/:clientId`

#### **Client Contract (2 endpoints)** ❌ **CRIAR**
- POST `/api/client-contract/upload`
- GET `/api/client-contract/:clientId`

#### **Client History (1 endpoint)** ❌ **CRIAR**
- GET `/api/client-history/:clientId`

#### **Gestores (5 endpoints)** ❌ **CRIAR**
- GET `/api/gestores`
- POST `/api/gestores`
- GET `/api/gestores/:id`
- PUT `/api/gestores/:id`
- DELETE `/api/gestores/:id`

#### **Design Materials (5 endpoints)** ❌ **CRIAR**
- POST `/api/design-materials`
- GET `/api/design-materials`
- GET `/api/design-materials/:id/download`
- GET `/api/design-materials/:id/share`
- GET `/api/design-materials/share/:token`

#### **Campaigns (2 endpoints)** ❌ **CRIAR**
- GET `/api/campaigns/active`
- GET `/api/campaigns`

#### **Páginas HTML (15 rotas)** 🔄 **USAR Manager.Web**
- GET `/` → Landing page
- GET `/manager` → Dashboard
- GET `/dashboard` → Dashboard
- GET `/portal` → Portal cliente
- GET `/index.html` → Login
- GET `/cadastro.html` → Cadastro
- GET `/contacts` → Contatos
- GET `/github` → GitHub
- GET `/payments` → Pagamentos
- etc.

---

## 🗄️ **2. Migração de Database**

### **Mudança crítica: PostgreSQL/SQLite → MongoDB Atlas**

#### **MongoDB Atlas - 3 Databases**
1. **avila_dashboard**
   - users
   - sessions
   - logs
   - config

2. **avila_gmail**
   - emails (3 contas)
   - threads
   - labels

3. **avila_crm**
   - leads
   - contacts (8000+)
   - interactions
   - campanhas

#### **Pacotes NuGet necessários**
```xml
<PackageReference Include="MongoDB.Driver" Version="3.3.0" />
<PackageReference Include="MongoDB.Bson" Version="3.3.0" />
```

#### **Remover EntityFramework Core**
- Manter estrutura de repositórios
- Trocar EF Core por MongoDB.Driver
- Adaptar queries LINQ

---

## 🔌 **3. Integrações a Migrar**

### **Pacotes NuGet necessários**

| Serviço | Pacote NuGet | Status |
|---------|--------------|--------|
| GitHub | `Octokit` | ✅ **JÁ INSTALADO** |
| Stripe | `Stripe.net` | ❌ **INSTALAR** |
| OpenAI | `OpenAI-DotNet` ou `Azure.AI.OpenAI` | ❌ **INSTALAR** |
| Gmail | `Google.Apis.Gmail.v1` | ❌ **INSTALAR** |
| MongoDB | `MongoDB.Driver` | ❌ **INSTALAR** |
| LinkedIn | API REST manual (HttpClient) | ❌ **CRIAR** |
| Meta/Facebook | `Meta.Business.Sdk` | ❌ **INSTALAR** |
| Nodemailer | `MailKit` ou `FluentEmail` | ❌ **INSTALAR** |
| Multer | `ASP.NET Core` (built-in) | ✅ **NATIVO** |
| JWT | `Microsoft.AspNetCore.Authentication.JwtBearer` | ✅ **JÁ INSTALADO** |

---

## 📅 CRONOGRAMA DE MIGRAÇÃO

### **FASE 1: Infraestrutura (1-2 dias)**
- [ ] Adicionar pacotes NuGet (MongoDB, Stripe, OpenAI, etc.)
- [ ] Configurar MongoDB Atlas no appsettings.json
- [ ] Criar repositórios MongoDB (substituir EF Core)
- [ ] Migrar middleware (CORS, rate limiting, JWT)
- [ ] Health checks

### **FASE 2: Core Business (2-3 dias)**
- [ ] CRM completo (leads, contacts, campanhas)
- [ ] Client management (finance, contracts, history)
- [ ] Gestores CRUD
- [ ] Backup/Export

### **FASE 3: Integrações Externas (3-4 dias)**
- [ ] GitHub API completa
- [ ] Stripe payments
- [ ] Gmail sync (3 contas)
- [ ] OpenAI (chat, images)
- [ ] LinkedIn automation
- [ ] Meta/Social (Instagram, Facebook, WhatsApp)

### **FASE 4: Features Especiais (2 dias)**
- [ ] E-Reader (biblioteca, diário, progresso)
- [ ] Calendar/Scheduling
- [ ] Health data tracking
- [ ] Financeiro

### **FASE 5: Frontend & Deploy (2-3 dias)**
- [ ] Manager.Web (Blazor ou Razor Pages)
- [ ] Servir arquivos estáticos
- [ ] Design materials upload
- [ ] Deploy no Render
- [ ] Testes end-to-end

### **FASE 6: Limpeza (1 dia)**
- [ ] Remover código Node.js
- [ ] Atualizar documentação
- [ ] CI/CD com .NET

---

## 🏗️ ARQUITETURA .NET FINAL

```
Admin/
├── src/
│   ├── Manager.Api/              # REST API (ASP.NET Core)
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs ✅
│   │   │   ├── CrmController.cs ❌
│   │   │   ├── GmailController.cs ❌
│   │   │   ├── GitHubController.cs 🔄
│   │   │   ├── StripeController.cs ❌
│   │   │   ├── OpenAIController.cs ❌
│   │   │   ├── LinkedInController.cs ❌
│   │   │   ├── MetaController.cs ❌
│   │   │   ├── CalendarController.cs ❌
│   │   │   ├── EReaderController.cs ❌
│   │   │   ├── FinanceController.cs ❌
│   │   │   └── ...
│   │   ├── Services/
│   │   │   ├── IAuthService.cs ✅
│   │   │   └── ...
│   │   └── Program.cs
│   │
│   ├── Manager.Core/              # Domain Layer
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Lead.cs
│   │   │   ├── Contact.cs
│   │   │   ├── Campaign.cs
│   │   │   ├── Book.cs
│   │   │   ├── DiaryEntry.cs
│   │   │   └── ...
│   │   └── Interfaces/
│   │
│   ├── Manager.Infrastructure/    # Data Layer (MongoDB)
│   │   ├── Data/
│   │   │   └── MongoDbContext.cs
│   │   ├── Repositories/
│   │   │   ├── IMongoRepository.cs
│   │   │   ├── LeadRepository.cs
│   │   │   ├── ContactRepository.cs
│   │   │   └── ...
│   │   └── MongoDB/
│   │
│   ├── Manager.Integrations/      # External Services
│   │   ├── GitHub/ ✅
│   │   ├── Google/ ✅
│   │   ├── Stripe/ ❌
│   │   ├── OpenAI/ ❌
│   │   ├── Gmail/ ❌
│   │   ├── LinkedIn/ ❌
│   │   └── Meta/ ❌
│   │
│   ├── Manager.Contracts/         # DTOs
│   │   └── DTOs/
│   │
│   ├── Manager.Web/               # Frontend
│   │   ├── Pages/ (Blazor/Razor)
│   │   ├── wwwroot/
│   │   │   ├── css/
│   │   │   ├── js/
│   │   │   └── pdf/ (Livros)
│   │   └── Program.cs
│   │
│   └── Manager.Worker/            # Background Jobs
│       ├── Jobs/
│       │   ├── EmailSyncJob.cs
│       │   ├── BackupJob.cs
│       │   └── ...
│       └── Program.cs
│
├── Admin.sln                      # Solution
├── appsettings.json
└── .env → appsettings.json        # Migrar secrets
```

---

## ⚙️ CONFIGURAÇÃO

### **appsettings.json**

```json
{
  "ConnectionStrings": {
    "MongoDbAtlas": "mongodb+srv://...",
    "Dashboard": "avila_dashboard",
    "Gmail": "avila_gmail",
    "Crm": "avila_crm"
  },
  "Integrations": {
    "GitHub": {
      "Token": "",
      "Owner": "avilaops"
    },
    "OpenAI": {
      "ApiKey": ""
    },
    "Stripe": {
      "ApiKey": "",
      "PublicKey": ""
    },
    "Gmail": {
      "ClientId": "",
      "ClientSecret": "",
      "Accounts": ["email1@gmail.com", "email2@gmail.com", "email3@gmail.com"]
    },
    "LinkedIn": {
      "ClientId": "",
      "ClientSecret": ""
    }
  },
  "Jwt": {
    "Secret": "",
    "Issuer": "admin.avila.inc",
    "Audience": "avila-clients",
    "ExpirationMinutes": 60
  }
}
```

---

## 🚀 PRÓXIMOS PASSOS IMEDIATOS

### **1. Preparar ambiente .NET**
```bash
cd d:\Projetos\Admin\src\Manager.Api
dotnet build
dotnet run
```

### **2. Adicionar pacotes necessários**
```bash
dotnet add package MongoDB.Driver
dotnet add package Stripe.net
dotnet add package OpenAI-DotNet
dotnet add package Google.Apis.Gmail.v1
dotnet add package MailKit
```

### **3. Criar estrutura MongoDB**
- MongoDbContext
- Repositories genéricos
- Collections typed

### **4. Começar migração por prioridade**
1. CRM (leads, contacts) - **CRÍTICO**
2. GitHub - **ALTA**
3. Gmail - **ALTA**
4. Stripe - **MÉDIA**
5. OpenAI - **BAIXA**

---

## ✅ CHECKLIST DE VALIDAÇÃO

- [ ] Todos os endpoints migrados
- [ ] MongoDB conectado e funcionando
- [ ] 23+ integrações funcionais
- [ ] Frontend servindo corretamente
- [ ] JWT e autenticação OK
- [ ] Upload de arquivos OK
- [ ] Background jobs configurados
- [ ] Health checks OK
- [ ] Deploy no Render
- [ ] Variáveis de ambiente configuradas
- [ ] Documentação atualizada
- [ ] Código Node.js removido

---

## 📊 ESFORÇO ESTIMADO

| Fase | Dias | Complexidade |
|------|------|--------------|
| Fase 1: Infraestrutura | 1-2 | Média |
| Fase 2: Core Business | 2-3 | Alta |
| Fase 3: Integrações | 3-4 | Alta |
| Fase 4: Features | 2 | Média |
| Fase 5: Frontend/Deploy | 2-3 | Alta |
| Fase 6: Limpeza | 1 | Baixa |
| **TOTAL** | **11-15 dias** | **Alta** |

---

## 🎯 RESULTADO ESPERADO

✅ Projeto 100% .NET 10  
✅ MongoDB Atlas como único database  
✅ Clean Architecture  
✅ Todas as 100+ APIs migradas  
✅ 23+ integrações funcionais  
✅ Frontend Blazor/Razor  
✅ Deploy no Render  
✅ Código Node.js removido  

---

**Status**: 📋 Plano criado - Aguardando aprovação para iniciar FASE 1
