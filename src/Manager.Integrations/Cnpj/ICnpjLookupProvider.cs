namespace Manager.Integrations.Cnpj;

public interface ICnpjLookupProvider
{
    Task<CnpjLookupResult> LookupAsync(string cnpj);
}

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
