# Gravatar API Integration - v0.1.0

## Visão Geral
API de integração com Gravatar para avatares de usuários no ecossistema Ávila Inc.

**Base URL:** `https://api.avila.inc/v0/gravatar`

**Versão:** 0.1.0  
**Data:** 2025-11-16

---

## Características

- ✅ Suporte a temas **claro** e **escuro**
- ✅ Batch requests (até 100 emails)
- ✅ Integração com 18 produtos Ávila
- ✅ Analytics integrado
- ✅ Fallback automático
- ✅ Versionamento semântico

---

## Endpoints

### 1. GET `/avatar/:email`
Obtém URL do avatar com configurações de tema.

**Query Parameters:**
- `size` (number, opcional): Tamanho do avatar (padrão: 200)
- `default` (string, opcional): Avatar padrão (mp, identicon, monsterid, wavatar, retro, robohash)
- `theme` (string, opcional): Tema - `light` ou `dark` (padrão: light)

**Headers:**
- `X-Product-Name` (string, opcional): Nome do produto fazendo a requisição

**Exemplo:**
```bash
GET https://api.avila.inc/v0/gravatar/avatar/user@example.com?size=200&theme=dark
```

**Resposta:**
```json
{
  "success": true,
  "version": "0.1.0",
  "data": {
    "email": "user@example.com",
    "avatarUrl": "https://www.gravatar.com/avatar/abc123...",
    "theme": "dark",
    "themeConfig": {
      "borderColor": "#333333",
      "backgroundColor": "#1a1a1a",
      "textColor": "#ffffff"
    },
    "hasCustomAvatar": true,
    "size": 200
  }
}
```

---

### 2. POST `/avatars/batch`
Obtém múltiplos avatares em uma única requisição.

**Body:**
```json
{
  "emails": ["user1@example.com", "user2@example.com"],
  "options": {
    "size": 80,
    "theme": "dark",
    "default": "identicon"
  }
}
```

**Resposta:**
```json
{
  "success": true,
  "version": "0.1.0",
  "data": {
    "avatars": {
      "user1@example.com": {
        "url": "https://www.gravatar.com/avatar/...",
        "theme": "dark",
        "themeConfig": { ... }
      },
      "user2@example.com": { ... }
    },
    "count": 2,
    "theme": "dark"
  }
}
```

---

### 3. GET `/product/:product/:email`
Obtém avatar formatado para produto específico.

**Path Parameters:**
- `product`: Nome do produto (secreta, dashboard, pulse, archivus, barbara, shancrys)
- `email`: Email do usuário

**Query Parameters:**
- `theme` (string, opcional): `light` ou `dark`

**Exemplo:**
```bash
GET https://api.avila.inc/v0/gravatar/product/secreta/user@example.com?theme=dark
```

**Resposta:**
```json
{
  "success": true,
  "version": "0.1.0",
  "data": {
    "email": "user@example.com",
    "product": "secreta",
    "avatarUrl": "https://www.gravatar.com/avatar/...",
    "theme": "dark",
    "themeConfig": {
      "borderColor": "#333333",
      "backgroundColor": "#1a1a1a",
      "textColor": "#ffffff"
    },
    "hasCustomAvatar": true,
    "timestamp": "2025-11-16T...",
    "fallback": null,
    "version": "0.1.0"
  }
}
```

---

### 4. GET `/profile/:email`
Obtém perfil completo do Gravatar.

**Exemplo:**
```bash
GET https://api.avila.inc/v0/gravatar/profile/user@example.com
```

---

### 5. GET `/check/:email`
Verifica se email possui Gravatar customizado.

**Resposta:**
```json
{
  "success": true,
  "data": {
    "email": "user@example.com",
    "hasGravatar": true
  }
}
```

---

### 6. GET `/hash/:email`
Obtém hash MD5 do email.

**Resposta:**
```json
{
  "success": true,
  "data": {
    "email": "user@example.com",
    "hash": "abc123def456..."
  }
}
```

---

## Configuração de Temas

### Light Theme (Padrão)
```json
{
  "borderColor": "#e5e5e5",
  "backgroundColor": "#ffffff",
  "textColor": "#1a1a1a"
}
```

### Dark Theme
```json
{
  "borderColor": "#333333",
  "backgroundColor": "#1a1a1a",
  "textColor": "#ffffff"
}
```

---

## Uso Frontend (JavaScript)

### Exemplo com Fetch API
```javascript
// GET Avatar com tema escuro
async function getAvatar(email, theme = 'light') {
  const response = await fetch(
    `https://api.avila.inc/v0/gravatar/avatar/${encodeURIComponent(email)}?theme=${theme}`,
    {
      headers: {
        'X-Product-Name': 'secreta'
      }
    }
  );
  
  const data = await response.json();
  return data.data;
}

// Uso
const avatar = await getAvatar('user@example.com', 'dark');
console.log(avatar.avatarUrl);
console.log(avatar.themeConfig);

// Batch request
async function getAvatarsBatch(emails, theme = 'light') {
  const response = await fetch(
    'https://api.avila.inc/v0/gravatar/avatars/batch',
    {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'X-Product-Name': 'dashboard'
      },
      body: JSON.stringify({
        emails,
        options: { size: 80, theme }
      })
    }
  );
  
  const data = await response.json();
  return data.data.avatars;
}

// Uso
const avatars = await getAvatarsBatch(
  ['user1@example.com', 'user2@example.com'],
  'dark'
);
```

---

## Componente React Exemplo

```jsx
import { useState, useEffect } from 'react';

function GravatarAvatar({ email, size = 80, theme = 'light' }) {
  const [avatar, setAvatar] = useState(null);
  
  useEffect(() => {
    fetch(`https://api.avila.inc/v0/gravatar/avatar/${encodeURIComponent(email)}?size=${size}&theme=${theme}`)
      .then(res => res.json())
      .then(data => setAvatar(data.data));
  }, [email, size, theme]);
  
  if (!avatar) return <div>Carregando...</div>;
  
  return (
    <div style={{
      display: 'inline-block',
      border: `2px solid ${avatar.themeConfig.borderColor}`,
      borderRadius: '50%',
      padding: '2px',
      backgroundColor: avatar.themeConfig.backgroundColor
    }}>
      <img 
        src={avatar.avatarUrl} 
        alt={email}
        style={{
          width: size,
          height: size,
          borderRadius: '50%',
          display: 'block'
        }}
      />
    </div>
  );
}

// Uso
<GravatarAvatar email="user@example.com" theme="dark" />
```

---

## Integração com Produtos Ávila

### Secreta (Sistema de Segurança)
```javascript
const avatar = await fetch(
  'https://api.avila.inc/v0/gravatar/product/secreta/admin@company.com?theme=dark'
).then(r => r.json());
```

### On Dashboard
```javascript
const avatar = await fetch(
  'https://api.avila.inc/v0/gravatar/product/dashboard/analyst@company.com?theme=light'
).then(r => r.json());
```

### Pulse Monitor
```javascript
const avatar = await fetch(
  'https://api.avila.inc/v0/gravatar/product/pulse/operator@company.com?theme=dark'
).then(r => r.json());
```

---

## Rate Limiting
- **Avatar individual:** 1000 req/min
- **Batch:** 100 req/min (máx 100 emails por request)
- **Profile:** 200 req/min

---

## Erros

### 400 Bad Request
```json
{
  "success": false,
  "error": "Array de emails é obrigatório"
}
```

### 404 Not Found
```json
{
  "success": false,
  "error": "Perfil não encontrado"
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "error": "Gravatar API error: 500"
}
```

---

## Changelog

### v0.1.0 (2025-11-16)
- ✨ Lançamento inicial
- ✨ Suporte a temas claro/escuro
- ✨ Integração com 18 produtos
- ✨ Batch requests
- ✨ Analytics integrado
- ✨ Versionamento semântico

---

## Próximas Versões

### v0.2.0 (Planejado)
- ⏳ Suporte a temas customizados
- ⏳ Cache Redis
- ⏳ WebSockets para atualizações em tempo real

### v0.3.0 (Planejado)
- ⏳ Upload de avatares customizados
- ⏳ Integração com Microsoft AD
- ⏳ SSO via OAuth

---

## Suporte

**Email:** dev@avila.inc  
**Documentação:** https://docs.avila.inc/gravatar  
**Status:** https://status.avila.inc
