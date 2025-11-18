# 📁 Estrutura do Projeto Ávila Inc

## Arquivos Principais

### 🌐 Website
- `index.html` - Página principal do site
- `CNAME` - Configuração de domínio customizado (avila.inc)
- `site.webmanifest` - Manifesto PWA

### 📄 Documentação
- `README.md` - Documentação principal
- `CONTRIBUTING.md` - Guia de contribuição
- `LICENSE` - Licença do projeto
- `docs/MIGRACAO_GITHUB_PAGES.md` - Guia de migração Azure → GitHub Pages
- `docs/GROK_VIDEO_INTEGRATION.md` - Integração de vídeos Grok
- `docs/QUICKSTART_VIDEO.md` - Guia rápido de geração de vídeos

### 🎨 Assets
- `assets/icons/` - Favicons e logos SVG personalizados
- `assets/images/` - Imagens e vídeos do site

### 🔧 Scripts
- `scripts/migrate-to-github-pages.js` - Script de migração DNS
- `scripts/generate_video.py` - Geração de vídeos com Grok AI
- `scripts/requirements.txt` - Dependências Python

### ⚙️ Configuração
- `.github/workflows/pages.yml` - Deploy automático GitHub Pages
- `.gitignore` - Arquivos ignorados pelo Git
- `.env` - Variáveis de ambiente (não commitado)

### 🗑️ API (Opcional)
- `api/` - Backend Node.js (email, gravatar)
  - Nota: Pode ser removida se não utilizada

## 🚀 Deploy

O site é automaticamente deployado no GitHub Pages quando há push na branch `main`.

**URL:** https://avila.inc

## 🧹 Limpeza Realizada

Arquivos removidos (obsoletos):
- ❌ `.github/workflows/azure-static-web-apps-*.yml` - Workflow Azure obsoleto
- ❌ `docs/CLOUDFLARE_SETUP.md` - Documentação obsoleta
- ❌ `docs/CLOUDFLARE_TOKEN.md` - Documentação obsoleta
- ❌ `docs/DOMAIN_SETUP.md` - Documentação obsoleta de DNS Azure
- ❌ `scripts/cloudflare-*.js` - Scripts DNS obsoletos (migração completa)
- ❌ `scripts/test-cloudflare.ps1` - Script de teste obsoleto

## 📝 Próximas Melhorias Sugeridas

1. **Avaliar pasta `api/`**: Se não estiver em uso, considerar remoção
2. **Otimizar vídeos**: Comprimir MP4s para carregamento mais rápido
3. **Adicionar sitemap.xml**: Para melhor SEO
4. **Adicionar robots.txt**: Para controle de crawlers
