namespace Manager.Contracts.DTOs;

/// <summary>
/// DTO para criação de lead público (Landing Page)
/// </summary>
public sealed record CreateLeadDto(
    string Name,
    string Email,
    string Phone,
    string? Message,
    string Source = "Landing",
    string? UtmSource = null,
    string? UtmCampaign = null,
    string? UtmMedium = null,
    string? PagePath = null,
    string? Referrer = null,
    string? ServiceInterest = null
);

/// <summary>
/// Resposta da criação de lead
/// </summary>
public sealed record CreateLeadResponse(
    string Id
);