# 🧪 Testes Automatizados - Integração GitHub

## 📋 Visão Geral

Suite completa de testes para garantir qualidade e estabilidade da integração GitHub.

## 🏗️ Estrutura de Testes

```
tests/
├── package.json                    # Configuração e dependências de testes
├── setup.ts                        # Setup global dos testes
├── unit/                           # Testes unitários
│   ├── components/
│   │   └── GitHubIntegration.test.tsx
│   └── routes/
│       └── github-routes.test.ts
└── integration/                    # Testes de integração
    └── github-flow.test.ts
```

## 🚀 Executar Testes

### Instalação

```bash
cd tests
npm install
```

### Comandos

```bash
# Todos os testes
npm test

# Com coverage
npm run test:coverage

# Watch mode (desenvolvimento)
npm run test:watch

# Apenas testes unitários
npm run test:unit

# Apenas testes de integração
npm run test:integration
```

## 📊 Coverage Atual

**Status:** 🔨 Em desenvolvimento

**Metas:**
- Componentes: 80%+
- Rotas API: 90%+
- Utilitários: 95%+
- Total: 85%+

## 🧪 Casos de Teste

### Frontend (GitHubIntegration.test.tsx)

✅ **Renderização:**
- Renderiza componente corretamente
- Exibe todas as abas
- Mostra loading state

✅ **Aba Overview:**
- Carrega informações do repositório
- Exibe erro quando falha

✅ **Aba Issues:**
- Lista issues
- Abre modal de criar issue
- Cria nova issue
- Valida campos obrigatórios

✅ **Aba Pull Requests:**
- Lista PRs
- Cria novo PR

✅ **Aba Branches:**
- Lista branches

✅ **Aba Buscar:**
- Busca código no repositório

✅ **Internacionalização:**
- Alterna entre pt-BR e en-US

✅ **Responsividade:**
- Layout mobile

### Backend (github-routes.test.ts)

✅ **GET /api/github/repository:**
- Retorna informações do repositório
- Trata erros da API

✅ **GET /api/github/issues:**
- Lista issues abertas por padrão
- Filtra por estado
- Valida parâmetros

✅ **POST /api/github/issues:**
- Cria nova issue
- Valida título obrigatório
- Valida dados

✅ **GET /api/github/pulls:**
- Lista pull requests
- Filtra por estado

✅ **POST /api/github/pulls:**
- Cria novo PR
- Valida campos obrigatórios
- Verifica branches existentes

✅ **GET /api/github/branches:**
- Lista branches
- Inclui commit SHA

✅ **GET /api/github/search/code:**
- Busca código
- Valida query obrigatória
- Retorna vazio se não encontrar

✅ **Segurança:**
- Sanitiza inputs (XSS prevention)
- Rate limiting
- CORS

### Integration (github-flow.test.ts)

✅ **Fluxo Completo:**
- Criar issue e atualizar lista
- Criar PR entre branches
- Buscar e encontrar código

✅ **Autenticação:**
- Funciona com token válido
- Falha graciosamente com token inválido

✅ **Performance:**
- Carrega em < 2 segundos
- Cacheia dados

✅ **Tratamento de Erros:**
- Mensagens amigáveis
- Retry em falhas

## 🔧 Configuração

### Jest Config (package.json)

```json
{
  "jest": {
    "preset": "ts-jest",
    "testEnvironment": "jsdom",
    "coverageThresholds": {
      "global": {
        "branches": 80,
        "functions": 80,
        "lines": 85,
        "statements": 85
      }
    }
  }
}
```

### Setup (setup.ts)

- Mock window.matchMedia
- Mock global fetch
- Setup @testing-library/jest-dom
- Reset mocks após cada teste

## 📈 Roadmap

### Fase 1: Setup ✅
- [x] Configuração Jest
- [x] Setup de testes
- [x] Estrutura de diretórios

### Fase 2: Testes Unitários 🔨
- [ ] Implementar testes de componentes
- [ ] Implementar testes de rotas
- [ ] Atingir 80% coverage

### Fase 3: Testes de Integração 📋
- [ ] Fluxos completos
- [ ] Testes E2E
- [ ] Performance tests

### Fase 4: CI/CD 📋
- [ ] GitHub Actions workflow
- [ ] Testes automáticos em PRs
- [ ] Coverage reports
- [ ] Quality gates

## 🐛 Troubleshooting

### Problema: "Cannot find module"

```bash
cd tests
rm -rf node_modules package-lock.json
npm install
```

### Problema: "Timeout in tests"

Aumentar timeout no Jest:
```typescript
jest.setTimeout(10000); // 10 segundos
```

### Problema: "Mock não está funcionando"

Verificar se mock está no setup.ts ou beforeEach:
```typescript
beforeEach(() => {
  global.fetch = jest.fn();
});
```

## 📚 Recursos

- [Jest Documentation](https://jestjs.io/docs/getting-started)
- [React Testing Library](https://testing-library.com/react)
- [Supertest](https://github.com/ladjs/supertest)
- [Testing Best Practices](https://testingjavascript.com/)

## 🤝 Contribuindo

1. Escreva testes para novas features
2. Mantenha coverage acima de 85%
3. Use describe/test descritivos
4. Mock dependências externas
5. Teste casos de erro

---

**Status:** 🔨 Em desenvolvimento
**Última atualização:** Dezembro 9, 2025
**Cobertura:** Estrutura criada, implementação em andamento
