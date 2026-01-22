using System.ComponentModel.DataAnnotations;
using Manager.Core.Enums;

namespace Manager.Contracts.DTOs;

public sealed record CreateWebsiteRequestDto
{
    [Required(ErrorMessage = "Nome do negócio é obrigatório")]
    [MinLength(2, ErrorMessage = "Nome deve ter pelo menos 2 caracteres")]
    [MaxLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string BusinessName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Nicho é obrigatório")]
    [MinLength(3, ErrorMessage = "Nicho deve ter pelo menos 3 caracteres")]
    [MaxLength(50, ErrorMessage = "Nicho deve ter no máximo 50 caracteres")]
    public string Niche { get; init; } = string.Empty;

    [Required(ErrorMessage = "Cidade é obrigatória")]
    [MinLength(3, ErrorMessage = "Cidade deve ter pelo menos 3 caracteres")]
    [MaxLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
    public string City { get; init; } = string.Empty;

    [Required(ErrorMessage = "Pelo menos um serviço é obrigatório")]
    [MinLength(1, ErrorMessage = "Informe pelo menos um serviço")]
    [MaxLength(10, ErrorMessage = "Máximo de 10 serviços")]
    public List<string> Services { get; init; } = new();

    [Required(ErrorMessage = "Diferencial é obrigatório")]
    [MinLength(10, ErrorMessage = "Diferencial deve ter pelo menos 10 caracteres")]
    [MaxLength(500, ErrorMessage = "Diferencial deve ter no máximo 500 caracteres")]
    public string Differentials { get; init; } = string.Empty;

    [Required(ErrorMessage = "WhatsApp é obrigatório")]
    [Phone(ErrorMessage = "WhatsApp inválido")]
    [RegularExpression(@"^\d{10,15}$", ErrorMessage = "WhatsApp deve conter apenas números (10-15 dígitos)")]
    public string WhatsApp { get; init; } = string.Empty;

    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Template é obrigatório")]
    public WebsiteTemplateType TemplateType { get; init; }

    [MaxLength(50)]
    public string? ColorPreference { get; init; }

    [Url(ErrorMessage = "URL da logo inválida")]
    public string? LogoUrl { get; init; }
}

public sealed record WebsiteRequestResponseDto
{
    public string Id { get; init; } = string.Empty;
    public string BusinessName { get; init; } = string.Empty;
    public WebsiteRequestStatus Status { get; init; }
    public string? PreviewUrl { get; init; }
    public string? LiveUrl { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed record WebsiteProjectDto
{
    public string Id { get; init; } = string.Empty;
    public string Subdomain { get; init; } = string.Empty;
    public string PreviewUrl { get; init; } = string.Empty;
    public string? LiveUrl { get; init; } = string.Empty;
    public bool IsPublished { get; init; }
    public WebsiteContentDto Content { get; init; } = new();
}

public sealed record WebsiteContentDto
{
    public string BusinessName { get; init; } = string.Empty;
    public string HeroHeadline { get; init; } = string.Empty;
    public string HeroSubheadline { get; init; } = string.Empty;
    public List<ServiceItemDto> Services { get; init; } = new();
    public List<string> Benefits { get; init; } = new();
    public List<FaqItemDto> FaqItems { get; init; } = new();
    public string WhatsApp { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

public sealed record ServiceItemDto(string Icon, string Title, string Description);
public sealed record FaqItemDto(string Question, string Answer);

public sealed record UpdateWebsiteContentDto
{
    public string? HeroHeadline { get; init; }
    public string? HeroSubheadline { get; init; }
    public List<ServiceItemDto>? Services { get; init; }
    public List<string>? Benefits { get; init; }
    public List<FaqItemDto>? FaqItems { get; init; }
}
