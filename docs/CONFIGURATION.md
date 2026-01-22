# Avx Manager - Environment Configuration

## Required Environment Variables

### Database
```bash
ConnectionStrings__DefaultConnection=Host=localhost;Database=manager;Username=postgres;Password=YOUR_PASSWORD
```

### Secrets Encryption
```bash
Secrets__EncryptionKey=CHANGE_THIS_TO_A_SECURE_32_CHAR_KEY_MINIMUM
```

### GitHub Integration
```bash
GitHub__Token=ghp_your_github_personal_access_token
GitHub__Owner=your-github-username-or-org
```

### Cloudflare (Optional - Phase 3)
```bash
Cloudflare__ApiToken=your_cloudflare_api_token
Cloudflare__ZoneId=your_zone_id
```

### CRM Integration (Optional - Phase 4)
```bash
CRM__Provider=AgencyOS
CRM__ApiKey=your_crm_api_key
```

### Payments (Optional - Phase 4)
```bash
Payments__Provider=Asaas
Payments__ApiKey=your_payment_api_key
```

## Development Setup

1. Copy `.env.example` to `.env`
2. Update all values
3. Run migrations: `dotnet ef database update -p src/Manager.Infrastructure -s src/Manager.Api`
4. Start API: `dotnet run --project src/Manager.Api`
5. Start Worker: `dotnet run --project src/Manager.Worker`
6. Start Web: `dotnet run --project src/Manager.Web`

## Production Deployment

Use Azure Key Vault, AWS Secrets Manager, or similar for production secrets.
Never commit `.env` or `appsettings.Production.json` with real secrets.
