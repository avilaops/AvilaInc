# Integração GitHub - AvilaInc

## 📋 Resumo

Foi criada uma integração completa com o repositório **avilaops/AvilaInc** no GitHub. Agora você pode gerenciar o projeto diretamente do site.

## ✨ Funcionalidades Implementadas

### Frontend

1. **Nova Aba GitHub no Menu**
   - Acesso direto à integração GitHub
   - Interface visual moderna e intuitiva

2. **Componente GitHubIntegration** (`frontend/components/GitHubIntegration.tsx`)
   - **Visão Geral**: Informações do repositório e ações rápidas
   - **Issues**: Visualizar e criar issues
   - **Pull Requests**: Gerenciar PRs
   - **Branches**: Listar todas as branches
   - **Arquivos**: Explorador de código (em desenvolvimento)

3. **Traduções i18n**
   - Suporte completo em Português (pt-BR) e Inglês (en-US)
   - Textos específicos para a integração GitHub

### Backend

1. **Rotas da API** (`backend/src/routes/github.ts`)
   - `GET /api/github/repository/:owner/:repo` - Informações do repositório
   - `GET /api/github/issues/:owner/:repo` - Listar issues
   - `POST /api/github/issues/:owner/:repo` - Criar issue
   - `GET /api/github/pulls/:owner/:repo` - Listar pull requests
   - `POST /api/github/pulls/:owner/:repo` - Criar pull request
   - `GET /api/github/branches/:owner/:repo` - Listar branches
   - `GET /api/github/contents/:owner/:repo/*` - Obter conteúdo de arquivos
   - `GET /api/github/search/code/:owner/:repo` - Buscar código

2. **Integração Octokit**
   - Cliente oficial do GitHub para Node.js
   - Autenticação via token (variável de ambiente)

## 🚀 Próximos Passos

### 1. Instalar Dependências

```bash
# Backend
cd backend
npm install

# Frontend (se necessário)
cd ../frontend
npm install
```

### 2. Configurar Token do GitHub

Crie um token de acesso pessoal no GitHub:
1. Acesse: https://github.com/settings/tokens
2. Clique em "Generate new token (classic)"
3. Selecione os escopos:
   - `repo` (acesso completo a repositórios)
   - `read:org` (ler organizações)
4. Copie o token gerado

Adicione no arquivo `.env` do backend:
```
GITHUB_TOKEN=seu_token_aqui
```

### 3. Iniciar a Aplicação

```bash
# Backend
cd backend
npm run dev

# Frontend (em outro terminal)
cd frontend
npm run dev
```

### 4. Acessar a Integração

1. Abra o navegador em `http://localhost:3000` (ou porta configurada)
2. Clique na aba "🐙 GitHub" no menu
3. Explore as funcionalidades!

## 🎯 Funcionalidades Disponíveis

- ✅ Visualizar informações do repositório avilaops/AvilaInc
- ✅ Listar e criar issues
- ✅ Listar e criar pull requests
- ✅ Visualizar branches
- ✅ Link direto para o GitHub
- ✅ Interface totalmente em português
- 🔄 Explorador de arquivos (em desenvolvimento)

## 🔒 Segurança

⚠️ **IMPORTANTE**: Nunca commite o arquivo `.env` com o token do GitHub. Adicione-o ao `.gitignore`.

## 📝 Notas Técnicas

- O componente faz chamadas à API do backend
- O backend usa o Octokit para comunicar com a API do GitHub
- Todas as requisições são autenticadas via token
- A interface é responsiva e segue o design system do projeto

## 🐛 Troubleshooting

Se encontrar erros de compilação TypeScript no backend:
```bash
cd backend
npm install --save-dev @types/express @types/node
```

---

**Desenvolvido com ❤️ para integração total com o projeto AvilaInc**
