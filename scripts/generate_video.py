#!/usr/bin/env python3
"""
Gerador de Vídeos Ávila Inc usando Grok Aurora API
Uso: python scripts/generate_video.py
"""

import os
import requests
import time
from pathlib import Path
from dotenv import load_dotenv

# Carrega variáveis de ambiente
load_dotenv()

class GrokVideoGenerator:
    """Classe para gerar vídeos usando a API Grok Aurora"""

    def __init__(self):
        self.api_key = os.getenv('XAI_API_KEY')
        if not self.api_key:
            raise ValueError("❌ XAI_API_KEY não encontrada no .env")

        self.base_url = os.getenv('XAI_BASE_URL', 'https://api.x.ai/v1')
        self.output_dir = Path(os.getenv('VIDEO_OUTPUT_DIR', './assets/images'))
        self.output_dir.mkdir(parents=True, exist_ok=True)

    def generate_video(self, prompt, filename, duration=5, size="1024x1024"):
        """
        Gera um vídeo usando Grok Aurora

        Args:
            prompt (str): Descrição detalhada do vídeo
            filename (str): Nome do arquivo de saída (ex: 'logo.mp4')
            duration (int): Duração em segundos (1-10)
            size (str): Resolução ('1024x1024', '1920x1080', etc)

        Returns:
            bool: True se sucesso, False se falhou
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

        print(f"\n🎬 Gerando vídeo: {filename}")
        print(f"📝 Prompt: {prompt[:100]}...")
        print(f"⏱️  Duração: {duration}s | 📐 Resolução: {size}")

        try:
            # Inicia geração
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
            print("⏳ Aguardando processamento...")

            # Aguarda processamento
            video_url = self._wait_for_video(video_id, headers)

            if video_url:
                # Download
                output_path = self._download_video(video_url, filename)
                print(f"🎉 Vídeo salvo em: {output_path}")
                return True
            else:
                print(f"❌ Falha ao gerar vídeo: {filename}")
                return False

        except requests.exceptions.RequestException as e:
            print(f"❌ Erro na API: {e}")
            if hasattr(e, 'response') and e.response is not None:
                print(f"   Detalhes: {e.response.text}")
            return False
        except Exception as e:
            print(f"❌ Erro inesperado: {e}")
            return False

    def _wait_for_video(self, video_id, headers, max_wait=300, poll_interval=10):
        """Aguarda o processamento do vídeo"""
        start_time = time.time()
        dots = 0

        while time.time() - start_time < max_wait:
            try:
                response = requests.get(
                    f'{self.base_url}/video/generations/{video_id}',
                    headers=headers,
                    timeout=30
                )
                response.raise_for_status()

                data = response.json()
                status = data.get('status', 'unknown')
                progress = data.get('progress', 0)

                if status == 'completed':
                    print(f"\n✅ Processamento concluído!")
                    return data.get('url') or data.get('video_url')

                elif status == 'failed':
                    error_msg = data.get('error', 'Erro desconhecido')
                    print(f"\n❌ Geração falhou: {error_msg}")
                    return None

                # Mostra progresso
                dots = (dots + 1) % 4
                dot_str = '.' * dots + ' ' * (3 - dots)
                elapsed = int(time.time() - start_time)
                print(f"\r⏳ {status.capitalize()}{dot_str} [{progress}%] - {elapsed}s", end='', flush=True)

                time.sleep(poll_interval)

            except requests.exceptions.RequestException as e:
                print(f"\n⚠️ Erro ao verificar status: {e}")
                time.sleep(poll_interval)

        print(f"\n⏰ Timeout após {max_wait}s")
        return None

    def _download_video(self, url, filename):
        """Faz download do vídeo gerado"""
        output_path = self.output_dir / filename

        print(f"📥 Baixando vídeo...")

        response = requests.get(url, stream=True, timeout=120)
        response.raise_for_status()

        total_size = int(response.headers.get('content-length', 0))
        downloaded = 0

        with open(output_path, 'wb') as f:
            for chunk in response.iter_content(chunk_size=8192):
                if chunk:
                    f.write(chunk)
                    downloaded += len(chunk)

                    if total_size > 0:
                        percent = (downloaded / total_size) * 100
                        print(f"\r📥 Download: {percent:.1f}%", end='', flush=True)

        print(f"\n💾 Tamanho: {downloaded / 1024 / 1024:.2f} MB")
        return output_path

def main():
    """Função principal - gera todos os vídeos do Ávila Inc"""
    print("=" * 60)
    print("🎬 Gerador de Vídeos Ávila Inc - Powered by Grok Aurora")
    print("=" * 60)

    generator = GrokVideoGenerator()

    videos = [
        {
            'prompt': (
                "Professional minimalist logo animation for tech company Ávila Inc. "
                "Elegant letter 'Á' with dark gradient from black to dark gray. "
                "Subtle pulse effect emanating from center in concentric circles. "
                "Clean corporate aesthetic, 4K quality, pure black background. "
                "Smooth fade-in with gentle 360-degree rotation, sophisticated and elegant."
            ),
            'filename': 'avila-logo-animation.mp4',
            'duration': 5
        },
        {
            'prompt': (
                "Modern tech dashboard animation showing real-time system monitoring. "
                "Vibrant blue gradient (from #2E5CFF to #C8DCFF) pulse waves flowing smoothly. "
                "Heartbeat visualization with clean ECG-style lines and data points. "
                "Multiple metrics connecting with light trails, futuristic interface. "
                "Dark navy background, professional corporate style, high-tech aesthetic, seamless loop."
            ),
            'filename': 'avila-pulse-monitor.mp4',
            'duration': 6
        },
        {
            'prompt': (
                "Artificial intelligence neural network visualization in motion. "
                "Purple gradient (from #6B46C1 to #9F7AEA) neural nodes connecting elegantly. "
                "Smooth data flow animation showing machine learning processes. "
                "Light particles traveling between nodes, forming complex patterns. "
                "Dark background, elegant and sophisticated tech aesthetic, corporate style."
            ),
            'filename': 'avila-ai-system.mp4',
            'duration': 5
        },
        {
            'prompt': (
                "Cybersecurity protection shield animation. "
                "Green gradient (from #059669 to #10B981) shield materializing from particles. "
                "Checkmark appearing inside shield with smooth animation. "
                "Data encryption particles flowing around, creating protective barrier. "
                "Dark background, professional security aesthetic, corporate style."
            ),
            'filename': 'avila-security.mp4',
            'duration': 5
        },
        {
            'prompt': (
                "3D BIM construction visualization animation. "
                "Orange gradient (from #D97706 to #F59E0B) wireframe building rotating. "
                "Architectural blueprints transforming into 3D model smoothly. "
                "Building layers appearing one by one with construction details. "
                "Dark background, professional architecture software aesthetic, modern tech style."
            ),
            'filename': 'avila-bim-construction.mp4',
            'duration': 6
        },
        {
            'prompt': (
                "Analytics dashboard with dynamic data visualization. "
                "Red gradient (from #DC2626 to #F87171) bar charts and graphs animating. "
                "Real-time metrics updating smoothly, data points connecting. "
                "Modern business intelligence interface with clean design. "
                "Dark background, professional corporate analytics style."
            ),
            'filename': 'avila-dashboard-analytics.mp4',
            'duration': 5
        }
    ]

    success_count = 0
    total_count = len(videos)

    for i, video in enumerate(videos, 1):
        print(f"\n{'=' * 60}")
        print(f"📹 Vídeo {i}/{total_count}")
        print(f"{'=' * 60}")

        if generator.generate_video(**video):
            success_count += 1

        # Pausa entre vídeos para respeitar rate limits
        if i < total_count:
            print("\n⏸️  Aguardando 30s antes do próximo vídeo...")
            time.sleep(30)

    print(f"\n{'=' * 60}")
    print(f"✅ Concluído: {success_count}/{total_count} vídeos gerados com sucesso!")
    print(f"📁 Vídeos salvos em: {generator.output_dir}")
    print(f"{'=' * 60}\n")

if __name__ == '__main__':
    try:
        main()
    except KeyboardInterrupt:
        print("\n\n⚠️ Operação cancelada pelo usuário")
    except Exception as e:
        print(f"\n\n❌ Erro fatal: {e}")
        import traceback
        traceback.print_exc()
