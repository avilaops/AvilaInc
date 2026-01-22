using Manager.Core.Entities;
using Manager.Core.Enums;
using Manager.Infrastructure.Repositories;
using Manager.Infrastructure.Services;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Manager.Worker.Services;

public interface IWebsiteGeneratorService
{
    Task<WebsiteProject?> GenerateWebsiteAsync(WebsiteRequest request);
}

public sealed class WebsiteGeneratorService : IWebsiteGeneratorService
{
    private readonly IWebsiteRequestRepository _requestRepo;
    private readonly IWebsiteProjectRepository _projectRepo;
    private readonly IOpenAIService _openAiService;
    private readonly ILogger<WebsiteGeneratorService> _logger;
    private readonly string _templatesPath;

    public WebsiteGeneratorService(
        IWebsiteRequestRepository requestRepo,
        IWebsiteProjectRepository projectRepo,
        IOpenAIService openAiService,
        ILogger<WebsiteGeneratorService> logger,
        IConfiguration configuration)
    {
        _requestRepo = requestRepo;
        _projectRepo = projectRepo;
        _openAiService = openAiService;
        _logger = logger;
        _templatesPath = configuration["WebsiteGenerator:TemplatesPath"] 
            ?? Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "templates");
    }

    public async Task<WebsiteProject?> GenerateWebsiteAsync(WebsiteRequest request)
    {
        try
        {
            _logger.LogInformation("Starting website generation for request {RequestId}", request.Id);

            // 1. Update status to Generating
            await _requestRepo.UpdateStatusAsync(request.Id!, WebsiteRequestStatus.Generating);

            // 2. Load template
            var templatePath = Path.Combine(_templatesPath, GetTemplateFolder(request.TemplateType));
            if (!Directory.Exists(templatePath))
            {
                throw new DirectoryNotFoundException($"Template not found: {templatePath}");
            }

            var templateConfig = await LoadTemplateConfigAsync(templatePath);

            // 3. Generate AI content
            var content = await GenerateContentWithAIAsync(request, templateConfig);

            // 4. Create project
            var subdomain = GenerateSubdomain(request.BusinessName);
            var project = new WebsiteProject
            {
                RequestId = request.Id!,
                Subdomain = subdomain,
                TemplateType = request.TemplateType,
                ThemeTokens = GetThemeTokens(request.ColorPreference, templateConfig),
                Content = content,
                Sections = templateConfig.Sections,
                PreviewUrl = $"https://preview.avila.inc/{subdomain}"
            };

            // 5. Render HTML
            var html = await RenderTemplateAsync(templatePath, project);
            project.GeneratedHtml = html;

            // 6. Render CSS
            var css = await RenderCssAsync(templatePath, project.ThemeTokens);
            project.GeneratedCss = css;

            // 7. Save project
            var savedProject = await _projectRepo.CreateAsync(project);

            // 8. Update request with project ID
            var updateFilter = MongoDB.Driver.Builders<WebsiteRequest>.Filter.Eq(x => x.Id, request.Id);
            var update = MongoDB.Driver.Builders<WebsiteRequest>.Update
                .Set(x => x.ProjectId, savedProject.Id)
                .Set(x => x.Status, WebsiteRequestStatus.ReadyForReview)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            await _requestRepo.UpdateStatusAsync(request.Id!, WebsiteRequestStatus.ReadyForReview);

            _logger.LogInformation("Website generation completed for request {RequestId}", request.Id);

            return savedProject;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating website for request {RequestId}", request.Id);
            await _requestRepo.UpdateStatusAsync(request.Id!, WebsiteRequestStatus.Failed, ex.Message);
            return null;
        }
    }

    private async Task<WebsiteContent> GenerateContentWithAIAsync(WebsiteRequest request, TemplateConfig config)
    {
        var prompt = $@"Você é um copywriter especializado em websites de negócios locais.

TAREFA: Gerar conteúdo para um website de {request.Niche} em {request.City}.

INFORMAÇÕES DO NEGÓCIO:
- Nome: {request.BusinessName}
- Nicho: {request.Niche}
- Cidade: {request.City}
- Serviços oferecidos: {string.Join(", ", request.Services)}
- Diferencial: {request.Differentials}

GERE O SEGUINTE CONTEÚDO (responda APENAS com JSON válido):

{{
  ""heroHeadline"": ""Headline principal (máx 80 chars, impactante, inclua o nome do negócio e cidade)"",
  ""heroSubheadline"": ""Subheadline (máx 120 chars, valor e diferencial)"",
  ""services"": [
    {{
      ""icon"": ""emoji"",
      ""title"": ""Nome do Serviço"",
      ""description"": ""Descrição curta (máx 100 chars)""
    }}
  ],
  ""benefits"": [
    ""Benefício 1 (máx 60 chars)"",
    ""Benefício 2"",
    ""Benefício 3"",
    ""Benefício 4""
  ],
  ""faqItems"": [
    {{
      ""question"": ""Pergunta frequente?"",
      ""answer"": ""Resposta clara e objetiva (máx 150 chars)""
    }}
  ],
  ""metaTitle"": ""Título SEO (máx 60 chars)"",
  ""metaDescription"": ""Descrição SEO (máx 160 chars)""
}}

REGRAS:
- Use tom profissional mas acessível
- Evite jargões e palavras complicadas
- Foque em benefícios, não features
- Seja específico sobre {request.City}
- Gere {request.Services.Count} serviços baseado na lista fornecida
- Gere 4 benefícios
- Gere 5 FAQ items
- Use emojis apropriados para os ícones dos serviços";

        var response = await _openAiService.GenerateTextAsync(prompt, 0.7, 2000);
        
        // Parse JSON response
        var jsonMatch = Regex.Match(response, @"\{[\s\S]*\}", RegexOptions.Multiline);
        if (!jsonMatch.Success)
        {
            throw new InvalidOperationException("OpenAI response did not contain valid JSON");
        }

        var aiContent = JsonSerializer.Deserialize<AIGeneratedContent>(jsonMatch.Value);
        if (aiContent == null)
        {
            throw new InvalidOperationException("Failed to deserialize AI response");
        }

        return new WebsiteContent
        {
            BusinessName = request.BusinessName,
            HeroHeadline = aiContent.HeroHeadline,
            HeroSubheadline = aiContent.HeroSubheadline,
            Services = aiContent.Services.Select(s => new ServiceItem
            {
                Icon = s.Icon,
                Title = s.Title,
                Description = s.Description
            }).ToList(),
            Benefits = aiContent.Benefits,
            FaqItems = aiContent.FaqItems.Select(f => new FaqItem
            {
                Question = f.Question,
                Answer = f.Answer
            }).ToList(),
            CtaText = $"Fale com {request.BusinessName} agora",
            WhatsApp = request.WhatsApp,
            Email = request.Email,
            City = request.City,
            MetaTitle = aiContent.MetaTitle,
            MetaDescription = aiContent.MetaDescription
        };
    }

    private async Task<string> RenderTemplateAsync(string templatePath, WebsiteProject project)
    {
        var templateHtml = await File.ReadAllTextAsync(Path.Combine(templatePath, "template.html"));

        // Simple template replacement (Handlebars-like)
        var rendered = templateHtml
            .Replace("{{businessName}}", project.Content.BusinessName)
            .Replace("{{heroHeadline}}", project.Content.HeroHeadline)
            .Replace("{{heroSubheadline}}", project.Content.HeroSubheadline)
            .Replace("{{ctaText}}", project.Content.CtaText)
            .Replace("{{whatsapp}}", project.Content.WhatsApp)
            .Replace("{{whatsappMessage}}", Uri.EscapeDataString($"Olá! Vim do site {project.Content.BusinessName}"))
            .Replace("{{email}}", project.Content.Email)
            .Replace("{{city}}", project.Content.City)
            .Replace("{{metaTitle}}", project.Content.MetaTitle)
            .Replace("{{metaDescription}}", project.Content.MetaDescription)
            .Replace("{{whatsappFormatted}}", FormatPhoneNumber(project.Content.WhatsApp))
            .Replace("{{year}}", DateTime.Now.Year.ToString());

        // Replace services (simple loop)
        var servicesHtml = new StringBuilder();
        foreach (var service in project.Content.Services)
        {
            servicesHtml.Append($@"
                <div class=""service-card"">
                    <div class=""service-icon"">{service.Icon}</div>
                    <h3>{service.Title}</h3>
                    <p>{service.Description}</p>
                </div>");
        }
        rendered = Regex.Replace(rendered, @"\{\{#each services\}\}.*?\{\{/each\}\}", servicesHtml.ToString(), RegexOptions.Singleline);

        // Replace benefits
        var benefitsHtml = new StringBuilder();
        foreach (var benefit in project.Content.Benefits)
        {
            benefitsHtml.Append($@"
                <div class=""benefit-item"">
                    <span class=""benefit-icon"">✅</span>
                    <span>{benefit}</span>
                </div>");
        }
        rendered = Regex.Replace(rendered, @"\{\{#each benefits\}\}.*?\{\{/each\}\}", benefitsHtml.ToString(), RegexOptions.Singleline);

        // Replace FAQ
        var faqHtml = new StringBuilder();
        foreach (var faq in project.Content.FaqItems)
        {
            faqHtml.Append($@"
                <div class=""faq-item"">
                    <h3 class=""faq-question"">{faq.Question}</h3>
                    <p class=""faq-answer"">{faq.Answer}</p>
                </div>");
        }
        rendered = Regex.Replace(rendered, @"\{\{#each faqItems\}\}.*?\{\{/each\}\}", faqHtml.ToString(), RegexOptions.Singleline);

        return rendered;
    }

    private async Task<string> RenderCssAsync(string templatePath, ThemeTokens tokens)
    {
        var templateCss = await File.ReadAllTextAsync(Path.Combine(templatePath, "style.css"));

        return templateCss
            .Replace("{{primaryColor}}", tokens.PrimaryColor)
            .Replace("{{secondaryColor}}", tokens.SecondaryColor)
            .Replace("{{accentColor}}", tokens.AccentColor)
            .Replace("{{fontFamily}}", tokens.FontFamily)
            .Replace("{{borderRadius}}", tokens.BorderRadius)
            .Replace("{{spacing}}", tokens.Spacing);
    }

    private ThemeTokens GetThemeTokens(string? colorPreference, TemplateConfig config)
    {
        if (!string.IsNullOrEmpty(colorPreference) && config.ColorPresets.ContainsKey(colorPreference))
        {
            var preset = config.ColorPresets[colorPreference];
            return new ThemeTokens
            {
                PrimaryColor = preset.PrimaryColor,
                SecondaryColor = preset.SecondaryColor,
                AccentColor = preset.AccentColor,
                FontFamily = config.DefaultTheme.FontFamily,
                BorderRadius = config.DefaultTheme.BorderRadius,
                Spacing = config.DefaultTheme.Spacing
            };
        }

        return new ThemeTokens
        {
            PrimaryColor = config.DefaultTheme.PrimaryColor,
            SecondaryColor = config.DefaultTheme.SecondaryColor,
            AccentColor = config.DefaultTheme.AccentColor,
            FontFamily = config.DefaultTheme.FontFamily,
            BorderRadius = config.DefaultTheme.BorderRadius,
            Spacing = config.DefaultTheme.Spacing
        };
    }

    private async Task<TemplateConfig> LoadTemplateConfigAsync(string templatePath)
    {
        var configPath = Path.Combine(templatePath, "template.json");
        var json = await File.ReadAllTextAsync(configPath);
        return JsonSerializer.Deserialize<TemplateConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Failed to load template config");
    }

    private string GetTemplateFolder(WebsiteTemplateType templateType)
    {
        return templateType switch
        {
            WebsiteTemplateType.ClinicaMedica => "clinica-medica",
            WebsiteTemplateType.EscritorioAdvocacia => "escritorio-advocacia",
            WebsiteTemplateType.Restaurante => "restaurante",
            WebsiteTemplateType.EcommerceLeve => "ecommerce-leve",
            WebsiteTemplateType.SalaoEstetica => "salao-estetica",
            WebsiteTemplateType.AcademiaFitness => "academia-fitness",
            WebsiteTemplateType.ConsultoriaEmpresarial => "consultoria-empresarial",
            WebsiteTemplateType.AgenciaMarketing => "agencia-marketing",
            _ => "clinica-medica"
        };
    }

    private string GenerateSubdomain(string businessName)
    {
        var slug = businessName.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("ç", "c")
            .Replace("ã", "a")
            .Replace("õ", "o")
            .Replace("á", "a")
            .Replace("é", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ú", "u");

        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
        slug = Regex.Replace(slug, @"-+", "-");

        return $"{slug}-{Guid.NewGuid().ToString("N")[..8]}";
    }

    private string FormatPhoneNumber(string phone)
    {
        if (phone.Length == 11)
        {
            return $"({phone[..2]}) {phone.Substring(2, 5)}-{phone.Substring(7)}";
        }
        return phone;
    }
}

// DTOs for AI response
public sealed record AIGeneratedContent
{
    public string HeroHeadline { get; init; } = "";
    public string HeroSubheadline { get; init; } = "";
    public List<AIServiceItem> Services { get; init; } = new();
    public List<string> Benefits { get; init; } = new();
    public List<AIFaqItem> FaqItems { get; init; } = new();
    public string MetaTitle { get; init; } = "";
    public string MetaDescription { get; init; } = "";
}

public sealed record AIServiceItem
{
    public string Icon { get; init; } = "";
    public string Title { get; init; } = "";
    public string Description { get; init; } = "";
}

public sealed record AIFaqItem
{
    public string Question { get; init; } = "";
    public string Answer { get; init; } = "";
}

public sealed record TemplateConfig
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Version { get; init; } = "";
    public string Description { get; init; } = "";
    public string Category { get; init; } = "";
    public List<string> Sections { get; init; } = new();
    public ThemeConfig DefaultTheme { get; init; } = new();
    public Dictionary<string, ThemeConfig> ColorPresets { get; init; } = new();
}

public sealed record ThemeConfig
{
    public string PrimaryColor { get; init; } = "";
    public string SecondaryColor { get; init; } = "";
    public string AccentColor { get; init; } = "";
    public string FontFamily { get; init; } = "";
    public string BorderRadius { get; init; } = "";
    public string Spacing { get; init; } = "";
}
