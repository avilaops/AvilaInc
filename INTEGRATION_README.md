# Avila Inc - Automation Platform with GitHub Integration

Platform de automação com integração completa ao GitHub para gerenciamento de repositórios, issues, pull requests e muito mais.

## 🚀 Features

- ✅ Integração completa com GitHub (repositórios, issues, PRs, branches, files)
- ✅ Interface moderna e responsiva
- ✅ Backend robusto com TypeScript e Express
- ✅ Frontend com React e TypeScript
- ✅ Suporte multi-idioma (pt-BR, en-US)
- ✅ Autenticação segura com GitHub
- ✅ API RESTful completa
- ✅ Docker support

## 📋 Prerequisites

- Node.js >= 16.0.0
- npm >= 8.0.0
- MongoDB (local ou cloud)
- GitHub Personal Access Token

## 🔧 Installation

1. Clone o repositório:
```bash
git clone https://github.com/avilaops/AvilaInc.git
cd AvilaInc
```

2. Instale as dependências:
```bash
npm install
```

3. Configure as variáveis de ambiente:
```bash
cp .env.example .env
```

Edite o arquivo `.env` com suas credenciais:
```env
GITHUB_OWNER=seu-usuario
GITHUB_REPO=seu-repositorio
GITHUB_TOKEN=seu-token-github
MONGODB_URI=mongodb://localhost:27017/avila-automation
PORT=3001
```

## 🏃 Running

### Development Mode

Backend:
```bash
npm run dev:backend
```

Frontend:
```bash
npm run dev:frontend
```

Todos os serviços:
```bash
npm run dev
```

### Production Mode

Build:
```bash
npm run build
```

Start:
```bash
npm start
```

## 🐳 Docker

Build e start com Docker Compose:
```bash
docker-compose up -d
```

Production:
```bash
docker-compose -f docker-compose.prod.yml up -d
```

## 📁 Project Structure

```
d:\Automation/
├── backend/
│   └── src/
│       ├── index.ts                    # Backend entry point
│       ├── routes/
│       │   ├── cases.ts               # Cases routes
│       │   ├── github-simple.ts       # Simple GitHub routes
│       │   └── github-integration.ts  # Full GitHub integration routes
│       ├── models/
│       │   ├── Case.ts
│       │   └── User.ts
│       └── services/
│           ├── CopilotAnalysisService.ts
│           ├── EmailService.ts
│           └── ProposalGeneratorService.ts
├── frontend/
│   ├── components/
│   │   ├── CaseForm.tsx
│   │   ├── CasesList.tsx
│   │   ├── LanguageSwitcher.tsx
│   │   ├── GitHubIntegration.tsx      # GitHub integration component
│   │   └── GitHubIntegration.css      # Styles
│   ├── pages/
│   │   ├── _app.tsx
│   │   ├── _document.tsx
│   │   └── index.tsx
│   └── hooks/
│       └── useI18n.ts
├── public/
│   ├── index.html                      # Landing page
│   └── github.html                     # GitHub integration page
├── i18n/
│   ├── en-US.json
│   └── pt-BR.json
├── packages/                           # Monorepo packages
├── .env.example
├── docker-compose.yml
├── package.json
└── README.md
```

## 🔗 API Endpoints

### GitHub Integration

#### Repository
- `GET /api/github/repository` - Get repository information

#### Issues
- `GET /api/github/issues` - List issues
- `POST /api/github/issues` - Create new issue

#### Pull Requests
- `GET /api/github/pulls` - List pull requests
- `POST /api/github/pulls` - Create new pull request

#### Branches
- `GET /api/github/branches` - List branches
- `POST /api/github/branches` - Create new branch

#### Files
- `GET /api/github/files?path=<path>` - List files in path
- `GET /api/github/file-content?path=<path>` - Get file content

### Cases (Existing)
- `GET /api/cases` - List all cases
- `POST /api/cases` - Create new case
- `GET /api/cases/:id` - Get case by ID
- `PUT /api/cases/:id` - Update case
- `DELETE /api/cases/:id` - Delete case

## 🎨 Frontend Components

### GitHubIntegration.tsx
Componente React completo com 5 tabs:
- **Overview**: Estatísticas do repositório
- **Issues**: Gerenciamento de issues
- **Pull Requests**: Gerenciamento de PRs
- **Branches**: Gerenciamento de branches
- **Files**: Navegação de arquivos

Funcionalidades:
- ✅ Criar issues, PRs e branches
- ✅ Listar e filtrar items
- ✅ Navegação em diretórios
- ✅ Design inspirado no GitHub
- ✅ Responsivo para mobile

## 🌍 Internationalization

Suporte para múltiplos idiomas através do `react-i18next`:

```typescript
import { useI18n } from '@/hooks/useI18n';

const { t, locale, setLocale } = useI18n();
```

Arquivos de tradução em `i18n/`:
- `en-US.json` - English
- `pt-BR.json` - Português

## 🔒 Security

- GitHub Personal Access Token para autenticação
- CORS configurado
- Validação de entrada em todas as rotas
- Sanitização de dados
- HTTPS recomendado em produção

## 📊 Technologies

### Backend
- Node.js
- Express
- TypeScript
- MongoDB + Mongoose
- CORS

### Frontend
- React 18+
- TypeScript
- Next.js
- CSS Modules
- react-i18next

### DevOps
- Docker
- Docker Compose
- Nginx

## 🤝 Contributing

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 License

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 👥 Authors

- **Avila Ops** - [avilaops](https://github.com/avilaops)

## 🙏 Acknowledgments

- GitHub API
- React Community
- TypeScript Team
- Node.js Community

## 📧 Contact

Para dúvidas ou sugestões, abra uma issue no repositório.

---

**Avila Inc** - Automation Platform with GitHub Integration 🚀
