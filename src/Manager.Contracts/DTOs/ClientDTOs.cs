namespace Manager.Contracts.DTOs;

public record CreateClientRequest(
    string Name,
    string? TaxId,
    string? Vertical
);

public record ClientResponse(
    Guid Id,
    string Name,
    string? TaxId,
    string? Vertical,
    bool IsActive,
    DateTime CreatedAt
);
