# 🤖 OpenAI API Integration

Integração completa com OpenAI API incluindo GPT-4, DALL-E 3, Embeddings e Moderation.

## 📦 Pacotes Instalados

- **Betalgo.OpenAI** 8.7.2 - SDK oficial para .NET

## ⚙️ Configuração

### appsettings.json

```json
{
  "Integrations": {
    "OpenAI": {
      "ApiKey": "sk-YOUR_OPENAI_API_KEY_HERE",
      "DefaultChatModel": "gpt-4-turbo",
      "DefaultImageModel": "dall-e-3"
    }
  }
}
```

**Importante:** Substitua `sk-YOUR_OPENAI_API_KEY_HERE` pela sua API key real do OpenAI.

## 🎯 Endpoints Disponíveis

### 1. Chat Simples
```http
POST /api/openai/chat
Authorization: Bearer {token}
Content-Type: application/json

{
  "message": "Explique o que é inteligência artificial",
  "systemMessage": "Você é um professor universitário",
  "model": "gpt-4-turbo"  // opcional
}
```

**Resposta:**
```json
{
  "success": true,
  "message": "Inteligência artificial é...",
  "model": "gpt-4-turbo",
  "usage": {
    "promptTokens": 15,
    "completionTokens": 120,
    "totalTokens": 135
  },
  "finishReason": "stop"
}
```

### 2. Chat com Histórico
```http
POST /api/openai/chat/completions
Authorization: Bearer {token}
Content-Type: application/json

{
  "messages": [
    { "role": "system", "content": "Você é um assistente prestativo" },
    { "role": "user", "content": "Qual a capital do Brasil?" },
    { "role": "assistant", "content": "A capital do Brasil é Brasília" },
    { "role": "user", "content": "E quantos habitantes tem?" }
  ],
  "model": "gpt-4-turbo",
  "temperature": 0.7,
  "maxTokens": 500
}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "id": "chatcmpl-123",
    "model": "gpt-4-turbo",
    "choices": [
      {
        "index": 0,
        "message": {
          "role": "assistant",
          "content": "Brasília tem aproximadamente 3,1 milhões de habitantes..."
        },
        "finishReason": "stop"
      }
    ],
    "usage": {
      "promptTokens": 42,
      "completionTokens": 28,
      "totalTokens": 70
    }
  }
}
```

### 3. Gerar Imagem (DALL-E 3)
```http
POST /api/openai/images/generate
Authorization: Bearer {token}
Content-Type: application/json

{
  "prompt": "A futuristic city with flying cars at sunset",
  "size": "1024x1024",  // opcional: 256x256, 512x512, 1024x1024
  "numberOfImages": 1,  // opcional: 1-10
  "model": "dall-e-3"   // opcional
}
```

**Resposta:**
```json
{
  "success": true,
  "count": 1,
  "images": [
    {
      "url": "https://oaidalleapiprodscus.blob.core.windows.net/...",
      "revisedPrompt": "A futuristic city with flying cars..."
    }
  ]
}
```

### 4. Editar Imagem
```http
POST /api/openai/images/edit
Authorization: Bearer {token}
Content-Type: multipart/form-data

image: [arquivo PNG]
prompt: "Add a rainbow in the sky"
mask: [arquivo PNG opcional - área transparente será editada]
```

**Resposta:**
```json
{
  "success": true,
  "count": 1,
  "images": [
    { "url": "https://..." }
  ]
}
```

### 5. Criar Variações de Imagem
```http
POST /api/openai/images/variations
Authorization: Bearer {token}
Content-Type: multipart/form-data

image: [arquivo PNG]
numberOfVariations: 3
```

**Resposta:**
```json
{
  "success": true,
  "count": 3,
  "images": [
    { "url": "https://..." },
    { "url": "https://..." },
    { "url": "https://..." }
  ]
}
```

### 6. Criar Embedding
```http
POST /api/openai/embeddings
Authorization: Bearer {token}
Content-Type: application/json

{
  "text": "Inteligência artificial está transformando o mundo",
  "model": "text-embedding-3-small"  // opcional
}
```

**Resposta:**
```json
{
  "success": true,
  "model": "text-embedding-3-small",
  "dimensions": 1536,
  "embedding": [0.0023, -0.0091, 0.0042, ...],
  "usage": {
    "promptTokens": 8,
    "totalTokens": 8
  }
}
```

### 7. Criar Embeddings em Batch
```http
POST /api/openai/embeddings/batch
Authorization: Bearer {token}
Content-Type: application/json

{
  "texts": [
    "Primeiro texto para embedding",
    "Segundo texto para embedding",
    "Terceiro texto para embedding"
  ],
  "model": "text-embedding-3-small"
}
```

**Resposta:**
```json
{
  "success": true,
  "model": "text-embedding-3-small",
  "count": 3,
  "embeddings": [
    {
      "index": 0,
      "embedding": [0.0023, -0.0091, ...],
      "dimensions": 1536
    },
    {
      "index": 1,
      "embedding": [0.0015, -0.0082, ...],
      "dimensions": 1536
    },
    {
      "index": 2,
      "embedding": [0.0031, -0.0095, ...],
      "dimensions": 1536
    }
  ],
  "usage": {
    "promptTokens": 24,
    "totalTokens": 24
  }
}
```

### 8. Moderar Texto
```http
POST /api/openai/moderation
Authorization: Bearer {token}
Content-Type: application/json

{
  "text": "Texto que você quer moderar para detectar conteúdo inapropriado"
}
```

**Resposta:**
```json
{
  "success": true,
  "flagged": false,
  "categories": {
    "hate": false,
    "hateThreatening": false,
    "harassment": false,
    "harassmentThreatening": false,
    "selfHarm": false,
    "selfHarmIntent": false,
    "selfHarmInstructions": false,
    "sexual": false,
    "sexualMinors": false,
    "violence": false,
    "violenceGraphic": false
  },
  "categoryScores": {
    "hate": 0.00012,
    "hateThreatening": 0.00001,
    "harassment": 0.00008,
    "harassmentThreatening": 0.00002,
    "selfHarm": 0.00001,
    "selfHarmIntent": 0.00001,
    "selfHarmInstructions": 0.00001,
    "sexual": 0.00015,
    "sexualMinors": 0.00001,
    "violence": 0.00010,
    "violenceGraphic": 0.00005
  }
}
```

### 9. Listar Modelos Disponíveis
```http
GET /api/openai/models
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "success": true,
  "total": 87,
  "models": {
    "gpt": [
      "gpt-4-turbo",
      "gpt-4",
      "gpt-3.5-turbo",
      "gpt-3.5-turbo-16k"
    ],
    "dalle": [
      "dall-e-3",
      "dall-e-2"
    ],
    "embeddings": [
      "text-embedding-3-large",
      "text-embedding-3-small",
      "text-embedding-ada-002"
    ],
    "all": ["...", "..."]
  }
}
```

## 🎁 Endpoints de Casos de Uso

### 10. Resumir Texto
```http
POST /api/openai/summarize
Authorization: Bearer {token}
Content-Type: application/json

{
  "text": "Texto longo que você quer resumir...",
  "maxWords": 100  // opcional, padrão: 100
}
```

**Resposta:**
```json
{
  "success": true,
  "summary": "Resumo conciso do texto...",
  "originalLength": 1523,
  "usage": {
    "promptTokens": 320,
    "completionTokens": 85,
    "totalTokens": 405
  }
}
```

### 11. Traduzir Texto
```http
POST /api/openai/translate
Authorization: Bearer {token}
Content-Type: application/json

{
  "text": "Hello, how are you?",
  "toLanguage": "Português",
  "fromLanguage": "Inglês"  // opcional, auto-detecta se omitido
}
```

**Resposta:**
```json
{
  "success": true,
  "translation": "Olá, como você está?",
  "fromLanguage": "Inglês",
  "toLanguage": "Português",
  "usage": {
    "promptTokens": 15,
    "completionTokens": 8,
    "totalTokens": 23
  }
}
```

## 🧪 Exemplos de Uso

### JavaScript/Fetch
```javascript
// Chat simples
const chatResponse = await fetch('http://localhost:5056/api/openai/chat', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    message: 'Qual é a diferença entre IA e Machine Learning?',
    systemMessage: 'Você é um especialista em tecnologia'
  })
});

const chatData = await chatResponse.json();
console.log(chatData.message);

// Gerar imagem
const imageResponse = await fetch('http://localhost:5056/api/openai/images/generate', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    prompt: 'A beautiful sunset over mountains',
    size: '1024x1024'
  })
});

const imageData = await imageResponse.json();
console.log('Image URL:', imageData.images[0].url);

// Criar embeddings para busca semântica
const embeddingResponse = await fetch('http://localhost:5056/api/openai/embeddings/batch', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    texts: [
      'Produto: iPhone 15 Pro Max',
      'Produto: Samsung Galaxy S24',
      'Produto: Google Pixel 8 Pro'
    ]
  })
});

const embeddingData = await embeddingResponse.json();
embeddingData.embeddings.forEach((item, i) => {
  console.log(`Embedding ${i}: ${item.dimensions} dimensões`);
});
```

### C# HttpClient
```csharp
using System.Net.Http.Json;

var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);

// Chat simples
var chatRequest = new
{
    message = "Explique Machine Learning em 3 parágrafos",
    systemMessage = "Você é um professor de ciência de dados"
};

var chatResponse = await client.PostAsJsonAsync(
    "http://localhost:5056/api/openai/chat", 
    chatRequest
);

var chatResult = await chatResponse.Content.ReadFromJsonAsync<ChatResponse>();
Console.WriteLine(chatResult.Message);

// Moderar conteúdo
var moderationRequest = new { text = "Texto para moderar" };
var moderationResponse = await client.PostAsJsonAsync(
    "http://localhost:5056/api/openai/moderation",
    moderationRequest
);

var moderationResult = await moderationResponse.Content
    .ReadFromJsonAsync<ModerationResponse>();

if (moderationResult.Flagged)
{
    Console.WriteLine("⚠️ Conteúdo inapropriado detectado!");
}
```

### Python Requests
```python
import requests

token = "your_jwt_token"
headers = {
    "Authorization": f"Bearer {token}",
    "Content-Type": "application/json"
}

# Chat com histórico
chat_data = {
    "messages": [
        {"role": "system", "content": "Você é um assistente útil"},
        {"role": "user", "content": "Qual a capital da França?"}
    ],
    "temperature": 0.7
}

response = requests.post(
    "http://localhost:5056/api/openai/chat/completions",
    headers=headers,
    json=chat_data
)

result = response.json()
print(result['data']['choices'][0]['message']['content'])

# Criar embeddings
embedding_data = {
    "text": "Inteligência Artificial é fascinante"
}

response = requests.post(
    "http://localhost:5056/api/openai/embeddings",
    headers=headers,
    json=embedding_data
)

result = response.json()
print(f"Embedding criado: {result['dimensions']} dimensões")
```

## 📊 Casos de Uso

### 1. Chatbot Inteligente
Use `/api/openai/chat/completions` para manter conversas contextualizadas com histórico.

### 2. Geração de Conteúdo
Use `/api/openai/chat` para gerar artigos, descrições de produtos, emails, etc.

### 3. Geração de Imagens
Use `/api/openai/images/generate` para criar ilustrações, logos, mockups.

### 4. Busca Semântica
1. Crie embeddings dos seus documentos com `/api/openai/embeddings/batch`
2. Armazene no MongoDB ou banco vetorial
3. Compare embeddings para encontrar conteúdo similar

### 5. Moderação de Conteúdo
Use `/api/openai/moderation` para filtrar comentários, reviews, posts.

### 6. Resumos Automáticos
Use `/api/openai/summarize` para criar resumos de artigos, documentos.

### 7. Tradução
Use `/api/openai/translate` para traduzir textos entre idiomas.

## 🔐 Segurança

- ✅ Todos os endpoints (exceto webhooks) exigem JWT Bearer token
- ✅ API Key armazenada em appsettings.json (use User Secrets em produção)
- ✅ Validação de entrada nos endpoints
- ✅ Tratamento de erros da API OpenAI
- ✅ Logging de operações e erros

## 💰 Custos

**Modelos de Chat:**
- GPT-4 Turbo: $0.01 / 1K tokens (input), $0.03 / 1K tokens (output)
- GPT-3.5 Turbo: $0.0005 / 1K tokens (input), $0.0015 / 1K tokens (output)

**Imagens (DALL-E 3):**
- 1024×1024: $0.040 por imagem
- 1024×1792 ou 1792×1024: $0.080 por imagem

**Embeddings:**
- text-embedding-3-small: $0.00002 / 1K tokens
- text-embedding-3-large: $0.00013 / 1K tokens

**Moderation:** Grátis

## 📝 Notas Importantes

1. **Rate Limits:** OpenAI tem rate limits por minuto/dia dependendo do seu plano
2. **Token Limits:** GPT-4 Turbo suporta até 128K tokens de contexto
3. **Imagens:** DALL-E 3 requer imagens PNG, máximo 4MB
4. **Embeddings:** Dimensões variam: small (1536), large (3072)
5. **Temperatura:** 0-2, onde 0 = determinístico, 2 = criativo/aleatório

## 🚀 Status

✅ **IMPLEMENTADO**
- OpenAIService (15 métodos)
- OpenAIController (11 endpoints)
- Chat completions (simples e avançado)
- Geração de imagens (DALL-E 3)
- Edição e variação de imagens
- Embeddings (single e batch)
- Moderação de conteúdo
- Listagem de modelos
- Casos de uso (resumo, tradução)

Total: **11 endpoints OpenAI** implementados
