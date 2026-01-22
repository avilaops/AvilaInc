namespace Manager.Contracts.DTOs;

public record CnpjLookupResult(
    string Cnpj,
    string RazaoSocial,
    string? NomeFantasia,
    string Status,
    DateTime? DataAbertura,
    string? CnaeMain,
    List<string> CnaeSecondary,
    string? Logradouro,
    string? Numero,
    string? Complemento,
    string? Bairro,
    string? Municipio,
    string? Uf,
    string? Cep,
    string? Telefone,
    string? Email,
    bool Success,
    string? ErrorMessage = null
);
