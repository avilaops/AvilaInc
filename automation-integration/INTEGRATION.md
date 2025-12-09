# 🔗 Integração com Plataforma de Automação

## 📋 Visão Geral

Esta branch contém a integração entre o site **AvilaInc** e a **Plataforma de Automação Client Strategy Analyzer**.

## 🎯 Objetivo

Permitir que usuários acessem e gerenciem o repositório AvilaInc diretamente através da plataforma de automação, incluindo:

- 📊 Visualização de informações do repositório
- 🐛 Gerenciamento de issues
- 🔀 Criação e gerenciamento de pull requests
- 🌿 Visualização de branches
- 📁 Navegação de arquivos
- 🔍 Busca de código

## 🚀 Funcionalidades Implementadas

### Frontend (React/TypeScript)

**Componente: `GitHubIntegration.tsx`**
- Interface visual moderna com Tailwind CSS
- Navegação por abas (Overview, Issues, PRs, Branches, Files)
- Integração com API backend
- Suporte a i18n (Português/Inglês)

### Backend (Node.js/Express)

**Rota: `/api/github`**
- Endpoints para repositório, issues, PRs, branches
- Integração com GitHub MCP
- Validação de dados
- Tratamento de erros

### Traduções (i18n)

**Arquivos:**
- `en-US.json` - Textos em inglês
- `pt-BR.json` - Textos em português

## 📦 Estrutura de Arquivos

```
automation-integration/
├── frontend/
│   └── components/
│       └── GitHubIntegration.tsx    # Componente React principal
├── backend/
│   └── routes/
│       └── github-simple.ts         # API endpoints
├── i18n/
│   ├── en-US.json                   # Traduções inglês
│   └── pt-BR.json                   # Traduções português
└── docs/
    └── INTEGRATION.md               # Esta documentação
```

## 🛠️ Instalação

### Backend

```bash
cd backend
npm install
```

### Frontend

```bash
cd frontend
npm install
```

## ⚙️ Configuração

O backend utiliza GitHub MCP (Model Context Protocol) para comunicação com a API do GitHub.

**Nenhuma configuração adicional necessária** - o MCP já está configurado no ambiente.

## 🎮 Uso

1. **Iniciar Backend:**
```bash
cd backend
npm run dev
```

2. **Iniciar Frontend:**
```bash
cd frontend
npm run dev
```

3. **Acessar a Aplicação:**
- Abra o navegador em `http://localhost:3000`
- Clique na aba "🐙 GitHub" no menu
- Explore as funcionalidades!

## 🔐 Segurança

- Todas as requisições são autenticadas via MCP
- Nenhum token exposto no código
- Validação de dados no backend
- CORS configurado adequadamente

## 📝 API Endpoints

### Repositório
- `GET /api/github/repository` - Informações do repositório

### Issues
- `GET /api/github/issues` - Listar issues
- `POST /api/github/issues` - Criar issue

### Pull Requests
- `GET /api/github/pulls` - Listar PRs
- `POST /api/github/pulls` - Criar PR

### Branches
- `GET /api/github/branches` - Listar branches

### Busca
- `GET /api/github/search/code?q=termo` - Buscar código

## 🧪 Testes

```bash
# Backend
cd backend
npm test

# Frontend
cd frontend
npm test
```

## 🎨 Design

- **Framework CSS:** Tailwind CSS
- **Paleta de cores:** Purple & Pink gradient
- **Responsivo:** Mobile-first
- **Acessibilidade:** WCAG 2.1 AA

## 📚 Tecnologias

### Frontend
- React 18+
- TypeScript
- Tailwind CSS
- Next.js

### Backend
- Node.js
- Express
- TypeScript
- GitHub MCP

## 🤝 Contribuindo

1. Fork o projeto
2. Crie sua branch (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](../LICENSE) para mais detalhes.

## 👥 Autores

- **AvilaOps Team** - Desenvolvimento e manutenção

## 🔗 Links Úteis

- [Repositório AvilaInc](https://github.com/avilaops/AvilaInc)
- [Documentação GitHub API](https://docs.github.com/en/rest)
- [GitHub MCP](https://github.com/modelcontextprotocol)

---

**Desenvolvido com ❤️ pela equipe AvilaOps**
