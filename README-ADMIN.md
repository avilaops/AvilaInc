# Avila Inc - Admin Dashboard

Painel administrativo da Avila Inc, hospedado no subdomínio manager.avila.inc.

## 🚀 Deploy no GitHub Pages

### Configuração Inicial

1. **Usar repositório existente**: `avilaops/manager`
2. **Configure o subdomínio**:
   - Vá para Settings > Pages
   - Em "Custom domain", coloque: `manager.avila.inc`
   - Salve as configurações

### Deploy Automático

O deploy é feito automaticamente via GitHub Actions quando você faz push na branch `main`.

### Deploy Manual

```bash
# Instalar dependências
npm install

# Build da aplicação
npm run build

# Deploy para GitHub Pages
npm run deploy
```

## 📁 Estrutura

```
avila-inc-admin/
├── src/
│   ├── views/
│   │   ├── index.html         # Login
│   │   ├── dashboard.html     # Dashboard principal
│   │   ├── portal-cliente.html # Portal do cliente
│   │   ├── cadastro.html      # Formulário de cadastro
│   │   └── ...
│   └── public/
│       ├── css/
│       ├── js/
│       └── ...
├── server.js                  # Backend (desenvolvimento local)
├── .github/
│   └── workflows/
│       └── deploy-admin.yml   # GitHub Actions
├── package-admin.json         # Configuração para deploy
└── README-ADMIN.md
```

## 🔗 URLs

- **Produção**: https://avilaops.github.io/avila-inc-admin/
- **Subdomínio**: https://manager.avila.inc (após configurar DNS)

## ⚠️ Importante

**Este é apenas o frontend estático!**

Para funcionalidades completas (backend, APIs, banco de dados), você precisa:

1. **Backend separado**: O `server.js` é para desenvolvimento local
2. **API externa**: Configure endpoints para produção
3. **Banco de dados**: MongoDB Atlas ou similar
4. **Autenticação**: JWT tokens, OAuth, etc.

### Configurações necessárias:

```javascript
// Em produção, configure:
const API_BASE_URL = 'https://api.avila.inc'; // Seu backend
const STRIPE_PUBLIC_KEY = 'pk_live_...'; // Chave de produção
```

## 📝 Funcionalidades

- ✅ Portal do cliente (`/portal`)
- ✅ Dashboard administrativo (`/manager`)
- ✅ Sistema de cadastro
- ✅ Integração Stripe (pagamentos)
- ✅ GitHub integration (via APIs)
- ✅ Interface responsiva

## 🔧 Desenvolvimento Local

```bash
# Instalar dependências
npm install

# Rodar servidor local
npm run dev

# Acesse: http://localhost:3000
```

## 📞 Suporte

Para dúvidas sobre deploy ou configuração, consulte a documentação completa em:
- [Documentação Técnica](docs/)
- [Guia de Setup](docs/SETUP.md)