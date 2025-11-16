# 🎨 Guia de Ícones Ávila Inc

## Ícones SVG Criados

### Favicon Principal
- **favicon-main.svg** - Favicon principal com "Á" (32x32px)
- **avila-logo.svg** - Logo básico com gradiente (100x100px)
- **apple-touch-icon-avila.svg** - Ícone para dispositivos Apple (180x180px)

### Ícones por Função/Produto

#### 1. **avila-pulse.svg** 🫀
- **Cor**: Azul (#2E5CFF → #C8DCFF)
- **Função**: Monitoramento em tempo real
- **Uso**: Pulse Monitor, Dashboards, Métricas
- **Elemento**: Gráfico de batimento cardíaco

#### 2. **avila-ai.svg** 🤖
- **Cor**: Roxo (#6B46C1 → #9F7AEA)
- **Função**: Inteligência Artificial
- **Uso**: Archivus AI, Machine Learning, Automação
- **Elemento**: Rosto de IA com smile

#### 3. **avila-security.svg** 🛡️
- **Cor**: Verde (#059669 → #10B981)
- **Função**: Segurança
- **Uso**: Secreta, Autenticação, Proteção
- **Elemento**: Escudo com check

#### 4. **avila-dashboard.svg** 📊
- **Cor**: Vermelho (#DC2626 → #F87171)
- **Função**: Analytics e Dashboards
- **Uso**: On Dashboard, Relatórios, Visualizações
- **Elemento**: Gráfico de barras

#### 5. **avila-bim.svg** 🏗️
- **Cor**: Laranja (#D97706 → #F59E0B)
- **Função**: Construção e BIM
- **Uso**: Shancrys BIM, Projetos 3D
- **Elemento**: Casa/Edifício

#### 6. **avila-api.svg** 🔌
- **Cor**: Ciano (#0891B2 → #06B6D4)
- **Função**: APIs e Integração
- **Uso**: Barbara API, Backend, Microserviços
- **Elemento**: Rede de conexões

## Uso nos HTMLs

Os ícones estão integrados em `index.html` e `index1.html`:

```html
<!-- Favicons Principais -->
<link rel="icon" type="image/svg+xml" href="assets/icons/favicon-main.svg">
<link rel="icon" type="image/svg+xml" sizes="16x16" href="assets/icons/avila-logo.svg">
<link rel="icon" type="image/svg+xml" sizes="32x32" href="assets/icons/avila-logo.svg">
<link rel="apple-touch-icon" href="assets/icons/apple-touch-icon-avila.svg">

<!-- Preload de Ícones Funcionais -->
<link rel="preload" as="image" href="assets/icons/avila-pulse.svg">
<link rel="preload" as="image" href="assets/icons/avila-ai.svg">
```

## Características dos SVGs

- ✅ Gradientes sofisticados
- ✅ Design minimalista
- ✅ Escaláveis (vetorial)
- ✅ Leves (< 2KB cada)
- ✅ Compatíveis com todos navegadores modernos
- ✅ Otimizados para dark/light mode

## Como Usar em Outros Contextos

### Como Imagem
```html
<img src="assets/icons/avila-pulse.svg" alt="Pulse Monitor" width="64">
```

### Como Background CSS
```css
.pulse-icon {
    background-image: url('assets/icons/avila-pulse.svg');
    background-size: contain;
}
```

### Inline no HTML
```html
<svg><!-- Copie o conteúdo do arquivo SVG aqui --></svg>
```

## Paleta de Cores

| Produto | Cor Primária | Cor Secundária | Uso |
|---------|--------------|----------------|-----|
| Logo | #1a1a1a | #4a4a4a | Identidade principal |
| Pulse | #2E5CFF | #C8DCFF | Monitoramento |
| AI | #6B46C1 | #9F7AEA | Inteligência |
| Security | #059669 | #10B981 | Proteção |
| Dashboard | #DC2626 | #F87171 | Analytics |
| BIM | #D97706 | #F59E0B | Construção |
| API | #0891B2 | #06B6D4 | Integração |

---

**Ávila Inc** - Design System v1.0
Última atualização: Novembro 2025
