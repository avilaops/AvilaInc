# Landing Page - Deploy

## 📦 Build para Produção

```bash
dotnet publish -c Release -o ./publish
```

## 🚀 Hospedagem

### IIS (Windows Server)
1. Instale ASP.NET Core Runtime 10.x
2. Copie conteúdo de `./publish` para `C:\inetpub\wwwroot\landing`
3. Crie site no IIS apontando para a pasta
4. Application Pool: No Managed Code

### Linux + Nginx
```bash
# Copiar arquivos
scp -r ./publish/* user@servidor:/var/www/landing

# Configurar systemd service
sudo nano /etc/systemd/system/landing.service
```

```ini
[Unit]
Description=Landing Page Blazor

[Service]
WorkingDirectory=/var/www/landing
ExecStart=/usr/bin/dotnet /var/www/landing/Landing.dll
Restart=always
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
```

```bash
sudo systemctl enable landing
sudo systemctl start landing
```

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY ./publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Landing.dll"]
```

```bash
docker build -t landing-page .
docker run -d -p 5000:5000 landing-page
```

## 🎨 Customização Rápida

### Trocar Número do WhatsApp
Editar `appsettings.WhatsApp.json`:
```json
{
  "WhatsApp": {
    "Phone": "5511999887766"
  }
}
```

### Atualizar OG Tags
Editar [Pages/_Host.cshtml](Pages/_Host.cshtml#L19-L23):
- `og:url` - Seu domínio
- `og:image` - Criar imagem 1200x630px em `wwwroot/images/og-image.png`

### Favicon
Colocar `favicon.ico` em `wwwroot/`

## ✅ Checklist Pré-Deploy

- [ ] Trocar telefone WhatsApp em `appsettings.WhatsApp.json`
- [ ] Atualizar `og:url` e `og:image` em `_Host.cshtml`
- [ ] Criar favicon.ico
- [ ] Testar em mobile/tablet/desktop
- [ ] Validar scroll reveal e contadores
- [ ] Verificar FAQ accordion (keyboard)
- [ ] Testar todos os links WhatsApp
- [ ] Configurar HTTPS no servidor
- [ ] CSP headers (opcional)
