# 🚀 Quick Start - Grok Video Integration

## Passo a Passo Rápido

### 1. Instalar Dependências
```bash
cd scripts
pip install -r requirements.txt
```

### 2. Configurar API Key
```bash
# Copie o exemplo
cp .env.example .env

# Edite o .env e adicione sua chave
# XAI_API_KEY=xai-sua-chave-aqui
```

### 3. Obter API Key do Grok
1. Acesse: https://console.x.ai/
2. Faça login ou crie conta
3. Vá em "API Keys"
4. Clique em "Create API Key"
5. Copie a chave (começa com `xai-`)
6. Cole no arquivo `.env`

### 4. Gerar Vídeos
```bash
# Gerar todos os vídeos
python scripts/generate_video.py

# Ou gerar um vídeo específico (edite o script)
```

### 5. Usar os Vídeos
Os vídeos serão salvos em `assets/images/` e já estão prontos para uso!

## Estrutura Criada

```
📦 Projeto
├── 📁 docs/
│   └── GROK_VIDEO_INTEGRATION.md  # Guia completo
├── 📁 scripts/
│   ├── generate_video.py          # Script gerador
│   └── requirements.txt           # Dependências
├── .env.example                   # Exemplo de configuração
└── .env                           # Suas credenciais (NÃO COMMITAR!)
```

## Prompts Prontos

No script `generate_video.py` já estão incluídos prompts otimizados para:
- ✅ Logo Ávila animado
- ✅ Pulse Monitor
- ✅ AI System
- ✅ Security Shield
- ✅ BIM Construction
- ✅ Dashboard Analytics

## Custos Estimados

- Cada vídeo (5-6s): ~$0.10 - $0.50
- 6 vídeos totais: ~$3.00 - $5.00
- Rate limit: ~10 vídeos/hora

## Troubleshooting

### "XAI_API_KEY não encontrada"
→ Certifique-se que o arquivo `.env` existe e tem a chave

### "Invalid API Key"
→ Verifique se copiou a chave completa do console xAI

### "Rate limit exceeded"
→ Aguarde alguns minutos, o script tem pausa automática de 30s

## Suporte

Documentação completa: `docs/GROK_VIDEO_INTEGRATION.md`
