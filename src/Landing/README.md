# Landing Page - Tech Solutions

Landing page moderna desenvolvida em **Blazor Server .NET 10** com renderização server-side, scroll reveal, contadores animados e accordion acessível.

## 🚀 Como Rodar

### Pré-requisitos
- .NET 10 SDK

### Executar Localmente
```bash
cd src/Landing
dotnet run
```

Acesse: `https://localhost:5001` ou `http://localhost:5000`

O projeto usa Blazor Server (não WebAssembly), com SignalR para comunicação em tempo real.

## 📱 WhatsApp

Para trocar o número de telefone do WhatsApp:

1. Abra `Components/Shared/Navbar.razor`
2. Abra `Components/Sections/HeroSection.razor`
3. Abra `Components/Sections/ServicesSection.razor`
4. Abra `Components/Sections/FinalCtaSection.razor`
5. Substitua `5511987654321` pelo novo número (com DDI+DDD sem espaços)

Exemplo: Para (21) 99999-8888, use: `552199999888`

## 🎨 Personalização

### Alterar Cores
Edite `wwwroot/css/app.css`:
```css
:root {
    --color-primary: #1976d2;    /* Azul principal */
    --color-secondary: #7c4dff;  /* Roxo secundário */
}
```

### Alterar Logo
Substitua o emoji 🚀 em:
- `Components/Shared/Navbar.razor`
- `Components/Sections/FooterSection.razor`

### Meta Tags (SEO)
Edite `Components/Pages/Index.razor`:
- `og:title`: Título para redes sociais
- `og:description`: Descrição para compartilhamento
- `og:url`: URL do site em produção
- `og:image`: Caminho da imagem (criar em `wwwroot/images/og-image.png`)

## 📦 Publicar (Production)

### Build Release
```bash
dotnet publish -c Release -o ./publish
```

### Deploy (IIS/Azure/AWS)
1. Copie pasta `publish/` para servidor
2. Configure como aplicação .NET 10
3. Defina variável de ambiente: `ASPNETCORE_ENVIRONMENT=Production`
4. Garanta HTTPS habilitado

### Deploy (Docker)
Crie `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Landing.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Landing.dll"]
```

Build e run:
```bash
docker build -t landing-page .
docker run -p 8080:80 landing-page
```

## 📝 Estrutura do Projeto

```
Landing/
├── Components/
│   ├── Layout/           # Layout principal
│   ├── Pages/            # Página Index (rota /)
│   ├── Sections/         # Seções da landing (Hero, Services, etc)
│   └── Shared/           # Componentes reutilizáveis (Navbar, BackToTop)
├── Models/               # Classes C# (ServiceCard, Testimonial, FaqItem)
├── wwwroot/
│   ├── css/app.css       # Estilos globais
│   ├── js/app.js         # JavaScript para interop
│   └── images/           # Imagens (adicionar aqui)
├── Program.cs            # Configuração do app
└── appsettings.json      # Configurações
```

## ✨ Funcionalidades

- ✅ Scroll suave entre seções
- ✅ Menu hamburger responsivo (mobile)
- ✅ Accordion FAQ interativo
- ✅ Animações on-scroll (IntersectionObserver)
- ✅ Botão "Voltar ao Topo"
- ✅ WhatsApp com mensagem pré-preenchida
- ✅ CSS isolado por componente
- ✅ Acessibilidade (ARIA, teclado, focus)
- ✅ Responsivo (Desktop 3 cols, Tablet 2, Mobile 1)
- ✅ SEO (meta tags, Open Graph)

## 🎯 Performance

- Sem bibliotecas externas pesadas (Bootstrap removido)
- CSS isolado por componente (scoped styles)
- JavaScript mínimo via JSInterop
- Respeita `prefers-reduced-motion`

## 📞 Contato de Exemplo

Todos os contatos são **placeholders**:
- WhatsApp: (11) 98765-4321 → Substituir pelo real
- Depoimentos marcados como "Exemplo" → Usar depoimentos reais

## 🛠️ Troubleshooting

### Erro ao rodar
```bash
dotnet clean
dotnet restore
dotnet build
dotnet run
```

### Animações não funcionam
Certifique-se que `wwwroot/js/app.js` está sendo carregado. Verifique console do navegador.

### WhatsApp não abre
Formato correto: `5511987654321` (DDI 55 + DDD + número sem espaços)

---

**Desenvolvido com Blazor .NET 10** 🚀
