# Sistema de Analytics - Avila Analytics

Sistema completo de analytics próprio para substituir Google Analytics, Facebook Pixel e Tawk.to.

## 📊 Funcionalidades

### 1. Tracking de Páginas
- Pageviews em tempo real
- Visitantes únicos
- Sessões de usuário
- Taxa de rejeição (bounce rate)
- Tempo médio de sessão
- Páginas mais visitadas

### 2. Eventos Customizados
- Rastreamento de cliques
- Eventos personalizados
- Conversões
- Metas e funis

### 3. Informações do Visitante
- Browser e sistema operacional
- Dispositivo (Mobile/Tablet/Desktop)
- Geolocalização (País/Cidade)
- Resolução de tela
- Referrer e UTM params

### 4. Dashboard em Tempo Real
- Visitantes ativos (últimos 5 minutos)
- Páginas acessadas recentemente
- Estatísticas do período selecionado

### 5. Chat ao Vivo (Futuro)
- Widget de chat integrado
- Mensagens em tempo real
- Histórico de conversas

## 🚀 Como Usar

### 1. Cadastrar um Site

Acesse **Sites** no menu lateral e clique em **Novo Site**:
- Nome: Nome do site do cliente
- Domínio: exemplo.com.br
- Chat: Habilitar ou não o widget de chat

### 2. Obter Código de Rastreamento

Após criar o site, clique no ícone **</>** (Code) para ver o snippet JavaScript:

```html
<script src="http://sua-api.com/analytics.js" 
        data-site-id="AVA_XXXXXXXXXXXXXXXX" 
        data-api-url="http://sua-api.com/api/analytics">
</script>
```

### 3. Instalar no Site do Cliente

Adicione o código acima no `<head>` do site do cliente. O tracking será automático!

### 4. Rastreamento Manual de Eventos

Para rastrear eventos personalizados, use a função global `avilaTrack`:

```javascript
// Evento simples
avilaTrack('botao_comprar_clicado');

// Evento com dados
avilaTrack('compra_realizada', {
    category: 'ecommerce',
    label: 'Produto XYZ',
    value: 199.90,
    metadata: {
        produto_id: '123',
        categoria: 'eletronicos'
    }
});
```

### 5. Ver Analytics

Acesse **Dashboard Analytics** no menu:
- Selecione o site
- Escolha o período (últimos 7, 30, 90 dias)
- Veja todas as métricas

## 📁 Arquivos Criados

### Backend (API)

1. **Analytics.cs** (`Manager.Core/Entities/Analytics.cs`)
   - Entidades: `PageView`, `Session`, `Visitor`, `AnalyticsEvent`, `Site`, `ChatMessage`

2. **AnalyticsController.cs** (`Manager.Api/Controllers/AnalyticsController.cs`)
   - POST `/api/analytics/track/pageview` - Rastrear visualização de página
   - POST `/api/analytics/track/event` - Rastrear evento customizado
   - GET `/api/analytics/dashboard/{siteId}` - Dados do dashboard
   - GET `/api/analytics/realtime/{siteId}` - Dados em tempo real

3. **SitesController.cs** (`Manager.Api/Controllers/SitesController.cs`)
   - CRUD completo para gerenciar sites
   - Geração de tracking codes

### Frontend (Blazor)

4. **Sites.razor** (`Manager.Web/Components/Pages/Sites.razor`)
   - Gerenciamento de sites
   - Visualização e cópia do código de rastreamento

5. **Analytics.razor** (`Manager.Web/Components/Pages/Analytics.razor`)
   - Dashboard com métricas
   - Atualização em tempo real a cada 10 segundos

### JavaScript Snippet

6. **analytics.js** (`public/analytics.js`)
   - Script de rastreamento leve (~3KB)
   - Auto-tracking de pageviews
   - Auto-tracking de cliques em links/botões
   - Detecção de dispositivo, browser, UTM params
   - Gerenciamento de sessões (30 min de inatividade)

## 🔧 Configurações MongoDB

As seguintes collections serão criadas automaticamente:
- `pageviews` - Visualizações de página
- `analytics_events` - Eventos customizados
- `sessions` - Sessões de usuários
- `visitors` - Visitantes únicos
- `sites` - Sites cadastrados
- `chat_messages` - Mensagens do chat

## 🎯 Benefícios

### vs Google Analytics
✅ Dados 100% seus (não compartilhados com Google)
✅ Sem cookies de terceiros (melhor para LGPD)
✅ Mais rápido (sem scripts externos pesados)
✅ Customização total

### vs Facebook Pixel
✅ Não depende de rede social
✅ Mais privacidade para os usuários
✅ Eventos customizados ilimitados

### vs Tawk.to
✅ Chat integrado ao sistema
✅ Histórico unificado com analytics
✅ Sem marca de terceiros

## 📈 Métricas Disponíveis

- **Total de Pageviews**: Quantas páginas foram vistas
- **Visitantes Únicos**: Quantas pessoas diferentes
- **Total de Sessões**: Quantas visitas ao site
- **Taxa de Rejeição**: % de visitantes que saíram sem interagir
- **Tempo Médio**: Quanto tempo ficam no site
- **Top Pages**: Páginas mais acessadas
- **Visitantes Ativos**: Em tempo real (últimos 5 min)

## 🔐 Segurança

- Tracking endpoints são públicos (`[AllowAnonymous]`)
- Dashboard endpoints requerem autenticação (`[Authorize]`)
- Validação de domínios permitidos
- Tracking codes únicos e regeneráveis

## 🚀 Próximos Passos

1. **Teste o sistema**:
   - Cadastre um site
   - Adicione o snippet em uma página HTML local
   - Veja os dados aparecerem no dashboard

2. **Chat ao Vivo** (implementar depois):
   - Widget flutuante no canto da página
   - SignalR para mensagens em tempo real
   - Notificações para agentes

3. **Relatórios Avançados**:
   - Exportar para PDF/Excel
   - Comparação de períodos
   - Alertas automáticos

## 📝 Exemplo de Uso Completo

```html
<!DOCTYPE html>
<html>
<head>
    <title>Meu Site</title>
    
    <!-- Avila Analytics -->
    <script src="http://localhost:5056/analytics.js" 
            data-site-id="AVA_ABC123456789" 
            data-api-url="http://localhost:5056/api/analytics">
    </script>
</head>
<body>
    <h1>Bem-vindo!</h1>
    
    <button onclick="avilaTrack('botao_contato', { category: 'cta', label: 'header' })">
        Entrar em Contato
    </button>
    
    <script>
        // Rastrear conversão quando formulário enviado
        document.getElementById('form').addEventListener('submit', function() {
            avilaTrack('formulario_enviado', {
                category: 'conversao',
                value: 1
            });
        });
    </script>
</body>
</html>
```

Agora você tem um **sistema completo de analytics** sem dependências externas! 🎉
