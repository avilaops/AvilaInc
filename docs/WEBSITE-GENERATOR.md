# 🚀 Sistema de Geração Automática de Sites - Avila.Inc

Sistema completo de geração de websites estáticos usando IA (OpenAI), com templates customizáveis e arquitetura production-ready.

## 📋 Visão Geral

O sistema permite que clientes criem websites profissionais em 3 minutos preenchendo um formulário simples. A IA gera conteúdo personalizado baseado em templates fixos, garantindo qualidade e escala.

### Fluxo Completo

```
Cliente preenche formulário (3 minutos)
           ↓
Landing → Manager.Api (valida + salva pedido)
           ↓
Worker processa fila (a cada 10s)
           ↓
OpenAI gera conteúdo (headlines, serviços, FAQ)
           ↓
Template rendering (HTML + CSS com placeholders)
           ↓
Preview disponível para cliente
           ↓
Cliente publica → Deploy para avila.inc
```

## 🏗️ Arquitetura

### Componentes

1. **Landing (Blazor Server)** - Frontend do wizard
   - Formulário 3 etapas
   - Preview do site
   - Controle de publicação

2. **Manager.Api** - REST API backend
   - CRUD de pedidos
   - Validação + sanitização
   - Rate limiting (3 pedidos/hora por IP)
   - CORS configurado para Landing

3. **Manager.Worker** - Background service
   - Processa fila de pedidos
   - Chama OpenAI para conteúdo
   - Renderiza templates
   - Gera HTML/CSS final

4. **Templates** - Estrutura versionada
   - `/templates/clinica-medica/`
   - `template.json` (config)
   - `template.html` (estrutura)
   - `style.css` (design tokens)

## 📂 Estrutura de Arquivos

### Novos Arquivos Criados

```
src/
├── Manager.Core/
│   ├── Entities/
│   │   ├── WebsiteRequest.cs        ✅ Pedido do cliente
│   │   ├── WebsiteProject.cs        ✅ Projeto gerado
│   │   └── WebsiteDeployment.cs     ✅ Deploy logs
│   └── Enums/
│       ├── WebsiteRequestStatus.cs  ✅ Status do pedido
│       └── WebsiteTemplateType.cs   ✅ Tipos de template
│
├── Manager.Contracts/
│   └── DTOs/
│       └── WebsiteDTOs.cs           ✅ DTOs de input/output
│
├── Manager.Infrastructure/
│   └── Repositories/
│       ├── WebsiteRequestRepository.cs    ✅
│       ├── WebsiteProjectRepository.cs    ✅
│       └── WebsiteDeploymentRepository.cs ✅
│
├── Manager.Api/
│   ├── Controllers/
│   │   └── PublicWebsiteController.cs     ✅ Endpoints públicos
│   └── Program.cs                          📝 Atualizado (CORS + repos)
│
├── Manager.Worker/
│   ├── Services/
│   │   └── WebsiteGeneratorService.cs     ✅ Lógica de geração
│   ├── BackgroundServices/
│   │   └── WebsiteGeneratorWorker.cs      ✅ Processador de fila
│   └── Program.cs                          📝 Atualizado (DI)
│
├── Landing/
│   ├── Pages/
│   │   ├── CriarSite.razor                ✅ Wizard 3 passos
│   │   └── MeuSite.razor                  ✅ Status + preview
│   ├── Services/
│   │   └── ManagerApiClient.cs            📝 Atualizado (novos métodos)
│   └── wwwroot/css/
│       └── app.css                         📝 Atualizado (wizard CSS)
│
└── templates/
    └── clinica-medica/
        ├── template.json     ✅ Configuração
        ├── template.html     ✅ Estrutura HTML
        └── style.css         ✅ Design tokens
```

## 🔌 Endpoints da API

### POST `/api/public/website-requests`
Cria novo pedido de website

**Request Body:**
```json
{
  "businessName": "Clínica Dr. Silva",
  "niche": "Clínica Médica",
  "city": "São Paulo - SP",
  "services": [
    "Consultas gerais",
    "Exames laboratoriais",
    "Check-up completo"
  ],
  "differentials": "Atendimento humanizado com 20 anos de experiência",
  "whatsapp": "5511987654321",
  "email": "contato@drsilva.com.br",
  "templateType": "ClinicaMedica",
  "colorPreference": "modern"
}
```

**Response (201):**
```json
{
  "id": "507f1f77bcf86cd799439011",
  "businessName": "Clínica Dr. Silva",
  "status": "Received",
  "createdAt": "2026-01-22T14:30:00Z",
  "updatedAt": "2026-01-22T14:30:00Z"
}
```

### GET `/api/public/website-requests/{id}`
Consulta status do pedido

**Response (200):**
```json
{
  "id": "507f1f77bcf86cd799439011",
  "businessName": "Clínica Dr. Silva",
  "status": "ReadyForReview",
  "previewUrl": "https://preview.avila.inc/clinica-dr-silva-a3f5b8c1",
  "liveUrl": null,
  "createdAt": "2026-01-22T14:30:00Z",
  "updatedAt": "2026-01-22T14:32:15Z"
}
```

### GET `/api/public/website-requests/{id}/preview`
Obtém preview do website gerado

**Response (200):**
```json
{
  "id": "507f1f77bcf86cd799439012",
  "subdomain": "clinica-dr-silva-a3f5b8c1",
  "previewUrl": "https://preview.avila.inc/clinica-dr-silva-a3f5b8c1",
  "liveUrl": "",
  "isPublished": false,
  "content": {
    "businessName": "Clínica Dr. Silva",
    "heroHeadline": "Clínica Dr. Silva - Cuidando da sua saúde em São Paulo",
    "heroSubheadline": "Atendimento humanizado com 20 anos de experiência",
    "services": [
      {
        "icon": "🩺",
        "title": "Consultas Gerais",
        "description": "Atendimento médico completo para todas as idades"
      }
    ],
    "benefits": [
      "Atendimento humanizado",
      "20 anos de experiência",
      "Equipe especializada",
      "Tecnologia de ponta"
    ],
    "faqItems": [
      {
        "question": "Como agendar consulta?",
        "answer": "Entre em contato pelo WhatsApp ou telefone"
      }
    ],
    "whatsapp": "5511987654321",
    "email": "contato@drsilva.com.br"
  }
}
```

### POST `/api/public/website-requests/{id}/publish`
Publica o website (torna público)

**Response (200):**
```json
{
  "message": "Website publicado com sucesso",
  "liveUrl": "https://clinica-dr-silva-a3f5b8c1.avila.inc"
}
```

## 🎨 Templates

### Estrutura de um Template

#### `template.json`
```json
{
  "id": "clinica-medica",
  "name": "Clínica Médica",
  "version": "1.0.0",
  "sections": ["hero", "services", "benefits", "faq", "cta", "footer"],
  "defaultTheme": {
    "primaryColor": "#0066cc",
    "secondaryColor": "#00994d",
    "accentColor": "#ff6600",
    "fontFamily": "Inter, sans-serif",
    "borderRadius": "12px",
    "spacing": "60px"
  },
  "colorPresets": {
    "modern": { "primaryColor": "#1976d2", ... },
    "clean": { "primaryColor": "#0066cc", ... },
    "sophisticated": { "primaryColor": "#2c3e50", ... }
  }
}
```

#### `template.html`
HTML com placeholders Handlebars-like:
- `{{businessName}}`
- `{{heroHeadline}}`
- `{{#each services}}...{{/each}}`

#### `style.css`
CSS com design tokens:
- `{{primaryColor}}`
- `{{secondaryColor}}`
- `{{borderRadius}}`

### Templates Disponíveis

1. **ClinicaMedica** 🏥 - Para clínicas e consultórios
2. **EscritorioAdvocacia** ⚖️ - Para advogados
3. **Restaurante** 🍽️ - Para delivery e restaurantes
4. **SalaoEstetica** 💇 - Para salões de beleza
5. **AcademiaFitness** 💪 - Para academias
6. **ConsultoriaEmpresarial** 📊 - Para consultorias B2B
7. **AgenciaMarketing** 📱 - Para agências digitais
8. **EcommerceLeve** 🛒 - Para e-commerce simples

## 🤖 Integração com OpenAI

### Prompt Usado

```
Você é um copywriter especializado em websites de negócios locais.

TAREFA: Gerar conteúdo para um website de [nicho] em [cidade].

INFORMAÇÕES DO NEGÓCIO:
- Nome: [businessName]
- Nicho: [niche]
- Cidade: [city]
- Serviços: [services]
- Diferencial: [differentials]

GERE (JSON):
{
  "heroHeadline": "...",
  "heroSubheadline": "...",
  "services": [...],
  "benefits": [...],
  "faqItems": [...],
  "metaTitle": "...",
  "metaDescription": "..."
}

REGRAS:
- Tone profissional mas acessível
- Evite jargões
- Foque em benefícios
- Seja específico sobre [city]
- Max 80 chars para headline
- Max 160 chars para meta description
```

## 🔒 Segurança

### Implementado

1. **Validação Server-Side**
   - DataAnnotations em DTOs
   - Regex para telefone/email
   - Length limits

2. **Sanitização**
   - Remove caracteres perigosos (< > ' ")
   - Normaliza phone numbers
   - Lowercase email

3. **Rate Limiting**
   - 3 pedidos/hora por IP
   - Verificação em `PublicWebsiteController`

4. **CORS**
   - Configurado para `http://localhost:5000` e `https://localhost:5000`
   - Produção: ajustar para domínio real

### TODO Produção

- [ ] Implementar rate limiting com Redis/Distributed Cache
- [ ] Adicionar captcha (hCaptcha/reCAPTCHA)
- [ ] Logging estruturado (Serilog)
- [ ] API Key para operações administrativas
- [ ] HTTPS obrigatório

## 🚀 Deploy

### Requisitos

- .NET 10.0
- MongoDB 7+
- OpenAI API Key

### Configuração

#### 1. `appsettings.json` (Manager.Api)
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseNames": {
      "Crm": "manager_crm"
    }
  },
  "OpenAI": {
    "ApiKey": "sk-..."
  },
  "Cors": {
    "AllowedOrigins": [
      "https://landing.avila.inc"
    ]
  }
}
```

#### 2. `appsettings.json` (Landing)
```json
{
  "ApiSettings": {
    "ManagerApiBaseUrl": "https://api.avila.inc/"
  }
}
```

#### 3. `appsettings.json` (Worker)
```json
{
  "WebsiteGenerator": {
    "TemplatesPath": "/app/templates"
  }
}
```

### Executar Localmente

```bash
# Terminal 1: Manager.Api
cd src/Manager.Api
dotnet run

# Terminal 2: Manager.Worker
cd src/Manager.Worker
dotnet run

# Terminal 3: Landing
cd src/Landing
dotnet run
```

Acessar: http://localhost:5000/criar-site

### Docker (Futuro)

```dockerfile
# Manager.Api + Manager.Worker
FROM mcr.microsoft.com/dotnet/aspnet:10.0
COPY publish/ /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "Manager.Api.dll"]
```

## 📊 Status do Sistema

### Status de Pedido

- **Received** - Pedido recebido, aguardando processamento
- **Generating** - IA gerando conteúdo
- **ReadyForReview** - Preview disponível para cliente
- **Published** - Site publicado e no ar
- **Failed** - Erro na geração

### Worker

- Polling: 10 segundos
- Batch: 5 pedidos por vez
- Retry: Implementar (TODO)

## 🎯 Próximos Passos

### Curto Prazo

- [ ] Endpoint de atualização de conteúdo (edição inline)
- [ ] Deploy real para GitHub Pages / Cloudflare Pages
- [ ] Preview iframe funcional
- [ ] Upload de logo
- [ ] Mais templates (restaurante, escritório advocacia)

### Médio Prazo

- [ ] Editor visual (arrastar seções)
- [ ] Banco de imagens (Unsplash API)
- [ ] Analytics integration
- [ ] Custom domain setup
- [ ] SEO automático (sitemap, robots.txt)

### Longo Prazo

- [ ] Painel admin com estatísticas
- [ ] Sistema de assinatura (planos)
- [ ] Multi-idioma
- [ ] A/B testing de templates
- [ ] Marketplace de templates

## 📝 Notas Técnicas

### Por que HTML estático?

1. **Performance** - 10x mais rápido que Blazor/React
2. **SEO** - Crawlers adoram HTML puro
3. **Custo** - Hospedagem gratuita/barata
4. **Escala** - 10.000 sites = 10.000 pastas, não servidores

### Por que Templates Fixos?

1. **Qualidade** - Design controlado, sempre profissional
2. **Manutenção** - Atualizar 1 template = atualizar 1000 sites
3. **Velocidade** - Rendering é instantâneo
4. **Escala** - Não vira "código aleatório de IA"

### Limitações da IA

- ✅ Gera textos (headlines, descrições, FAQ)
- ✅ Sugere cores baseado em paleta
- ❌ **NÃO** gera HTML do zero
- ❌ **NÃO** gera CSS do zero
- ❌ **NÃO** adiciona seções fora do template

## 📞 Suporte

Dúvidas sobre o sistema? Entre em contato:
- **GitHub**: github.com/avilaops/avilainc
- **Domínio**: avila.inc

---

**Desenvolvido com ❤️ por Avila.Inc** 🚀
