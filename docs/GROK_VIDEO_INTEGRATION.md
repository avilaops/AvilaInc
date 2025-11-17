# 🎬 Integração Grok para Geração de Vídeos

## Visão Geral

O Grok da xAI oferece capacidade de gerar vídeos através da API Aurora. Este guia mostra como integrar e automatizar a geração de vídeos para o projeto Ávila Inc.

## 1. Configuração Inicial

### Obter API Key
1. Acesse: https://console.x.ai/
2. Crie uma conta ou faça login
3. Navegue para **API Keys**
4. Gere uma nova chave: `xai-...`
5. Guarde em local seguro (nunca commite no git)

### Instalar Dependências

```bash
# Python
pip install requests python-dotenv openai

# Node.js
npm install axios dotenv form-data
```

## 2. Configuração de Ambiente

Crie um arquivo `.env` na raiz do projeto:

```env
# xAI / Grok API
XAI_API_KEY=xai-seu-token-aqui
XAI_BASE_URL=https://api.x.ai/v1

# Configurações de Vídeo
VIDEO_OUTPUT_DIR=./assets/images
VIDEO_DEFAULT_DURATION=5
VIDEO_DEFAULT_SIZE=1024x1024
```

Adicione `.env` ao `.gitignore`:
```bash
echo ".env" >> .gitignore
```

## 3. API Endpoints Disponíveis

### Gerar Vídeo (Aurora)
```
POST https://api.x.ai/v1/video/generations
```

**Headers:**
```json
{
  "Authorization": "Bearer xai-your-api-key",
  "Content-Type": "application/json"
}
```

**Body:**
```json
{
  "model": "aurora-video-1.0",
  "prompt": "Professional corporate logo animation with pulse effect",
  "duration": 5,
  "size": "1024x1024",
  "quality": "standard",
  "style": "corporate"
}
```

## 4. Script Python de Integração

Crie `scripts/generate_video.py`:

```python
import os
import requests
import time
from dotenv import load_dotenv
from pathlib import Path

load_dotenv()

class GrokVideoGenerator:
    def __init__(self):
        self.api_key = os.getenv('XAI_API_KEY')
        self.base_url = os.getenv('XAI_BASE_URL', 'https://api.x.ai/v1')
        self.output_dir = Path(os.getenv('VIDEO_OUTPUT_DIR', './assets/images'))

    def generate_video(self, prompt, filename, duration=5, size="1024x1024"):
        """
        Gera um vídeo usando Grok Aurora

        Args:
            prompt (str): Descrição do vídeo desejado
            filename (str): Nome do arquivo de saída
            duration (int): Duração em segundos
            size (str): Resolução do vídeo
        """
        headers = {
            'Authorization': f'Bearer {self.api_key}',
            'Content-Type': 'application/json'
        }

        payload = {
            'model': 'aurora-video-1.0',
            'prompt': prompt,
            'duration': duration,
            'size': size,
            'quality': 'hd',
            'style': 'corporate-professional'
        }

        print(f"🎬 Gerando vídeo: {filename}")
        print(f"📝 Prompt: {prompt}")

        try:
            # Requisição para gerar vídeo
            response = requests.post(
                f'{self.base_url}/video/generations',
                headers=headers,
                json=payload,
                timeout=60
            )
            response.raise_for_status()

            data = response.json()
            video_id = data.get('id')

            print(f"✅ Vídeo iniciado! ID: {video_id}")

            # Aguardar processamento
            video_url = self._wait_for_video(video_id, headers)

            if video_url:
                # Download do vídeo
                self._download_video(video_url, filename)
                print(f"🎉 Vídeo salvo: {filename}")
                return True

        except requests.exceptions.RequestException as e:
            print(f"❌ Erro na API: {e}")
            return False

    def _wait_for_video(self, video_id, headers, max_wait=300):
        """Aguarda o vídeo ser processado"""
        start_time = time.time()

        while time.time() - start_time < max_wait:
            try:
                response = requests.get(
                    f'{self.base_url}/video/generations/{video_id}',
                    headers=headers
                )
                response.raise_for_status()

                data = response.json()
                status = data.get('status')

                if status == 'completed':
                    return data.get('url')
                elif status == 'failed':
                    print(f"❌ Geração falhou: {data.get('error')}")
                    return None

                print(f"⏳ Status: {status}... aguardando")
                time.sleep(10)

            except requests.exceptions.RequestException as e:
                print(f"⚠️ Erro ao verificar status: {e}")
                time.sleep(10)

        print("⏰ Timeout: vídeo não foi gerado no tempo esperado")
        return None

    def _download_video(self, url, filename):
        """Faz download do vídeo gerado"""
        self.output_dir.mkdir(parents=True, exist_ok=True)
        output_path = self.output_dir / filename

        response = requests.get(url, stream=True)
        response.raise_for_status()

        with open(output_path, 'wb') as f:
            for chunk in response.iter_content(chunk_size=8192):
                f.write(chunk)

# Uso
if __name__ == '__main__':
    generator = GrokVideoGenerator()

    # Vídeo 1: Logo Ávila com animação
    generator.generate_video(
        prompt="Professional corporate logo animation for tech company Ávila Inc. "
               "Elegant letter 'Á' with gradient, subtle pulse effect, dark background, "
               "minimalist and sophisticated style, 4K quality",
        filename="avila-logo-animation.mp4",
        duration=5
    )

    # Vídeo 2: Pulse Monitor
    generator.generate_video(
        prompt="Real-time monitoring dashboard animation with pulse waves, "
               "heartbeat visualization, data flowing, blue gradient theme, "
               "futuristic tech interface, smooth transitions, corporate style",
        filename="avila-pulse-monitor.mp4",
        duration=6
    )

    # Vídeo 3: AI System
    generator.generate_video(
        prompt="Artificial intelligence neural network animation, "
               "purple gradient nodes connecting, data processing visualization, "
               "machine learning concept, elegant and sophisticated",
        filename="avila-ai-system.mp4",
        duration=5
    )
```

## 5. Script Node.js Alternativo

Crie `scripts/generate_video.js`:

```javascript
import axios from 'axios';
import fs from 'fs';
import path from 'path';
import dotenv from 'dotenv';
import { pipeline } from 'stream/promises';

dotenv.config();

class GrokVideoGenerator {
  constructor() {
    this.apiKey = process.env.XAI_API_KEY;
    this.baseUrl = process.env.XAI_BASE_URL || 'https://api.x.ai/v1';
    this.outputDir = process.env.VIDEO_OUTPUT_DIR || './assets/images';
  }

  async generateVideo(prompt, filename, duration = 5, size = '1024x1024') {
    const headers = {
      'Authorization': `Bearer ${this.apiKey}`,
      'Content-Type': 'application/json'
    };

    const payload = {
      model: 'aurora-video-1.0',
      prompt,
      duration,
      size,
      quality: 'hd',
      style: 'corporate-professional'
    };

    console.log(`🎬 Gerando vídeo: ${filename}`);
    console.log(`📝 Prompt: ${prompt}`);

    try {
      const response = await axios.post(
        `${this.baseUrl}/video/generations`,
        payload,
        { headers }
      );

      const videoId = response.data.id;
      console.log(`✅ Vídeo iniciado! ID: ${videoId}`);

      const videoUrl = await this.waitForVideo(videoId, headers);

      if (videoUrl) {
        await this.downloadVideo(videoUrl, filename);
        console.log(`🎉 Vídeo salvo: ${filename}`);
        return true;
      }
    } catch (error) {
      console.error(`❌ Erro: ${error.message}`);
      return false;
    }
  }

  async waitForVideo(videoId, headers, maxWait = 300000) {
    const startTime = Date.now();

    while (Date.now() - startTime < maxWait) {
      try {
        const response = await axios.get(
          `${this.baseUrl}/video/generations/${videoId}`,
          { headers }
        );

        const { status, url, error } = response.data;

        if (status === 'completed') return url;
        if (status === 'failed') {
          console.error(`❌ Geração falhou: ${error}`);
          return null;
        }

        console.log(`⏳ Status: ${status}... aguardando`);
        await new Promise(resolve => setTimeout(resolve, 10000));
      } catch (error) {
        console.warn(`⚠️ Erro ao verificar: ${error.message}`);
        await new Promise(resolve => setTimeout(resolve, 10000));
      }
    }

    console.log('⏰ Timeout');
    return null;
  }

  async downloadVideo(url, filename) {
    const outputPath = path.join(this.outputDir, filename);

    if (!fs.existsSync(this.outputDir)) {
      fs.mkdirSync(this.outputDir, { recursive: true });
    }

    const response = await axios({
      method: 'get',
      url,
      responseType: 'stream'
    });

    await pipeline(
      response.data,
      fs.createWriteStream(outputPath)
    );
  }
}

// Uso
const generator = new GrokVideoGenerator();

await generator.generateVideo(
  'Professional corporate logo animation for Ávila Inc',
  'avila-logo.mp4'
);
```

## 6. Automação com GitHub Actions

Crie `.github/workflows/generate-videos.yml`:

```yaml
name: Generate Avila Videos

on:
  workflow_dispatch:
    inputs:
      video_type:
        description: 'Tipo de vídeo'
        required: true
        type: choice
        options:
          - logo
          - pulse
          - ai
          - all

jobs:
  generate:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup Python
        uses: actions/setup-python@v4
        with:
          python-version: '3.11'

      - name: Install dependencies
        run: |
          pip install requests python-dotenv

      - name: Generate Videos
        env:
          XAI_API_KEY: ${{ secrets.XAI_API_KEY }}
        run: |
          python scripts/generate_video.py

      - name: Commit videos
        run: |
          git config user.name "Avila Bot"
          git config user.email "bot@avila.inc"
          git add assets/images/*.mp4
          git commit -m "🎬 chore: Update generated videos"
          git push
```

## 7. Prompts Otimizados para Ávila Inc

### Logo Animation
```
Professional minimalist logo animation for tech company Ávila Inc.
Elegant letter 'Á' with dark gradient (black to dark gray).
Subtle pulse effect emanating from center.
Clean corporate aesthetic, 4K quality, dark background.
Smooth fade-in with gentle rotation, duration 5 seconds.
```

### Pulse Monitor
```
Modern tech dashboard animation showing real-time monitoring.
Blue gradient pulse waves flowing across screen.
Heartbeat visualization with ECG-style lines.
Data points connecting and flowing smoothly.
Futuristic interface, clean corporate style, high-tech feel.
```

### AI System
```
Artificial intelligence neural network visualization.
Purple gradient nodes connecting with light trails.
Smooth data flow animation showing machine learning.
Elegant and sophisticated tech aesthetic.
Professional corporate style, dark background, seamless loop.
```

### BIM Construction
```
3D building information modeling visualization.
Orange gradient wireframe building rotating smoothly.
Construction plans materializing into 3D model.
Professional architecture software interface.
Clean corporate aesthetic, modern tech style.
```

### Security Shield
```
Cybersecurity protection animation.
Green gradient shield icon with checkmark materializing.
Data encryption particles flowing around shield.
Secure lock mechanism engaging smoothly.
Professional corporate security aesthetic.
```

## 8. Boas Práticas

### ✅ DO
- Use prompts detalhados e específicos
- Especifique estilo corporativo/profissional
- Mencione duração e qualidade desejada
- Salve vídeos em formato web-friendly (MP4)
- Use compressão adequada para web
- Teste localmente antes de committar

### ❌ DON'T
- Não commite API keys no repositório
- Não gere vídeos muito longos (>10s)
- Não use resolução excessiva para web
- Não ignore otimização de tamanho
- Não abuse da API (respeite rate limits)

## 9. Otimização de Vídeos

Após gerar, comprima os vídeos:

```bash
# Usando FFmpeg
ffmpeg -i input.mp4 -c:v libx264 -crf 23 -preset medium -c:a aac -b:a 128k output.mp4

# Redimensionar para web
ffmpeg -i input.mp4 -vf scale=1920:1080 -c:v libx264 -crf 23 output.mp4

# Converter para WebM (alternativa)
ffmpeg -i input.mp4 -c:v libvpx-vp9 -crf 30 -b:v 0 output.webm
```

## 10. Custos e Limites

**xAI Aurora Video:**
- ~$0.10 - $0.50 por vídeo (5-10s)
- Rate limit: ~10 vídeos/hora
- Max duration: 10 segundos
- Max resolution: 1920x1080

## 11. Troubleshooting

### Erro: "Invalid API Key"
- Verifique se a chave está correta no `.env`
- Confirme que a chave não expirou no console

### Erro: "Rate limit exceeded"
- Aguarde alguns minutos antes de tentar novamente
- Implemente retry com backoff exponencial

### Vídeo não carrega no navegador
- Certifique-se que está em formato MP4/H.264
- Adicione atributos `playsinline muted autoplay loop`
- Verifique tamanho do arquivo (<5MB recomendado)

## 12. Recursos Adicionais

- **Documentação xAI**: https://docs.x.ai/
- **Aurora Video API**: https://docs.x.ai/api/aurora
- **FFmpeg Docs**: https://ffmpeg.org/documentation.html
- **Web Video Guidelines**: https://web.dev/video/

---

**Ávila Inc** - Guia de Integração Grok v1.0
Última atualização: Novembro 2025
