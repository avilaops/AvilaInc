# 🔄 Conversão de Ícones para PNG

Este documento orienta a conversão dos SVGs para PNG quando necessário.

## Opções de Conversão

### 1. Online (Recomendado para rapidez)
- **CloudConvert**: https://cloudconvert.com/svg-to-png
- **Convertio**: https://convertio.co/svg-png/
- **SVG to PNG**: https://svgtopng.com/

### 2. Usando ImageMagick (Linha de comando)
```bash
# Instalar ImageMagick
# Windows: winget install ImageMagick.ImageMagick
# Linux: sudo apt install imagemagick

# Converter individualmente
magick assets/icons/avila-logo.svg -resize 512x512 assets/icons/avila-logo-512.png
magick assets/icons/avila-pulse.svg -resize 192x192 assets/icons/avila-pulse-192.png

# Converter todos de uma vez
for file in assets/icons/*.svg; do
  magick "$file" -resize 512x512 "${file%.svg}-512.png"
done
```

### 3. Usando Inkscape (GUI)
```bash
# Instalar Inkscape
# Windows: winget install Inkscape.Inkscape

# CLI Export
inkscape assets/icons/avila-logo.svg --export-type=png --export-width=512 --export-filename=assets/icons/avila-logo-512.png
```

### 4. Script PowerShell (Windows)
```powershell
# Usando Inkscape via PowerShell
$icons = Get-ChildItem -Path "assets/icons" -Filter "*.svg"
foreach ($icon in $icons) {
    $output = $icon.FullName -replace ".svg", "-512.png"
    & "C:\Program Files\Inkscape\bin\inkscape.exe" $icon.FullName --export-type=png --export-width=512 --export-filename=$output
}
```

## Tamanhos Recomendados para PWA

| Tamanho | Uso |
|---------|-----|
| 32x32 | Favicon padrão |
| 192x192 | Android Chrome |
| 512x512 | Android Chrome (alta resolução) |
| 180x180 | Apple Touch Icon |
| 96x96 | Atalhos PWA |

## Após Converter

Atualize o `site.webmanifest` para incluir versões PNG:

```json
"icons": [
  {
    "src": "/assets/icons/avila-logo.svg",
    "sizes": "512x512",
    "type": "image/svg+xml",
    "purpose": "any maskable"
  },
  {
    "src": "/assets/icons/avila-logo-512.png",
    "sizes": "512x512",
    "type": "image/png",
    "purpose": "any maskable"
  },
  {
    "src": "/assets/icons/avila-logo-192.png",
    "sizes": "192x192",
    "type": "image/png",
    "purpose": "any"
  }
]
```

## Otimização PNG

Após gerar os PNGs, otimize-os:

```bash
# Usando OptiPNG
optipng -o7 assets/icons/*.png

# Usando PNGQuant (melhor compressão)
pngquant --quality=80-95 assets/icons/*.png
```

---

**Nota**: Os SVGs são preferíveis por serem vetoriais e escaláveis, mas PNGs podem ser necessários para compatibilidade com navegadores mais antigos.
