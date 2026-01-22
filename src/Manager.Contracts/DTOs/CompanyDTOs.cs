namespace Manager.Contracts.DTOs;

public record CompanyDto
{
    public string Id { get; init; } = string.Empty;
    public string Source { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? GooglePlaceId { get; init; }
    public string? Cnpj { get; init; }
    public string? FantasyName { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? ZipCode { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Website { get; init; }
    public string? Types { get; init; }
    public double? Rating { get; init; }
    public int? UserRatingsTotal { get; init; }
    public string? BusinessStatus { get; init; }
    public string? CadastralStatus { get; init; }
    public DateTime? OpeningDate { get; init; }
    public string? CnaeMain { get; init; }
    public List<string>? CnaeSecondary { get; init; }
    public DateTime? LastSyncedAt { get; init; }
    public DateTime? LastCnpjLookupAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreateCompanyDto
{
    public required string Source { get; init; }
    public required string Name { get; init; }
    public string? GooglePlaceId { get; init; }
    public string? Address { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string? Phone { get; init; }
    public string? Website { get; init; }
    public string? Types { get; init; }
    public double? Rating { get; init; }
    public int? UserRatingsTotal { get; init; }
    public string? BusinessStatus { get; init; }
    public string? RawJson { get; init; }
    public string? Cnpj { get; init; }
}

public record UpdateCompanyDto
{
    public string? Name { get; init; }
    public string? Address { get; init; }
    public string? Phone { get; init; }
    public string? Website { get; init; }
    public string? Cnpj { get; init; }
}

public record GooglePlacesSearchDto
{
    public required string Query { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public int? RadiusMeters { get; init; } = 5000;
    public int? MaxResults { get; init; } = 20;
}

public record GooglePlaceResultDto
{
    public required string PlaceId { get; init; }
    public required string Name { get; init; }
    public string? Address { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string? Phone { get; init; }
    public string? Website { get; init; }
    public string? Types { get; init; }
    public double? Rating { get; init; }
    public int? UserRatingsTotal { get; init; }
    public string? BusinessStatus { get; init; }
    public string? RawJson { get; init; }
}

public record ImportCompaniesDto
{
    public required List<string> PlaceIds { get; init; }
}

public record CnpjLookupDto
{
    public required string Cnpj { get; init; }
}

public record CnpjLookupResultDto
{
    public required string Cnpj { get; init; }
    public string? RazaoSocial { get; init; }
    public string? NomeFantasia { get; init; }
    public string? SituacaoCadastral { get; init; }
    public DateTime? DataAbertura { get; init; }
    public string? CnaePrincipal { get; init; }
    public string? CnaesSecundarios { get; init; }
    public string? Logradouro { get; init; }
    public string? Numero { get; init; }
    public string? Complemento { get; init; }
    public string? Bairro { get; init; }
    public string? Municipio { get; init; }
    public string? Uf { get; init; }
    public string? Cep { get; init; }
    public string? Telefone { get; init; }
    public string? Email { get; init; }
    public string? RawJson { get; init; }
}