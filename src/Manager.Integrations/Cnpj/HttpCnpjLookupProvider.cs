using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Manager.Integrations.Cnpj;

public class HttpCnpjLookupProvider : ICnpjLookupProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpCnpjLookupProvider> _logger;
    private readonly string _baseUrl;
    private readonly string? _apiKey;

    public HttpCnpjLookupProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<HttpCnpjLookupProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // ReceitaWS é grátis e não precisa de key (rate limit 3 req/min)
        _baseUrl = configuration["CnpjLookup:BaseUrl"] ?? "https://receitaws.com.br/v1/cnpj";
        _apiKey = configuration["CnpjLookup:ApiKey"];
    }

    public async Task<CnpjLookupResult> LookupAsync(string cnpj)
    {
        try
        {
            var cleanCnpj = NormalizeCnpj(cnpj);
            
            if (!IsValidCnpj(cleanCnpj))
            {
                return new CnpjLookupResult(
                    cleanCnpj,
                    string.Empty,
                    null,
                    "INVALID",
                    null,
                    null,
                    new List<string>(),
                    null, null, null, null, null, null, null, null, null,
                    false,
                    "CNPJ inválido"
                );
            }

            var url = $"{_baseUrl}/{cleanCnpj}";
            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            }

            _logger.LogInformation("Consultando CNPJ: {Cnpj}", MaskCnpj(cleanCnpj));

            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Erro ao consultar CNPJ: {StatusCode} - {Error}", response.StatusCode, error);
                
                return new CnpjLookupResult(
                    cleanCnpj,
                    string.Empty,
                    null,
                    "ERROR",
                    null,
                    null,
                    new List<string>(),
                    null, null, null, null, null, null, null, null, null,
                    false,
                    $"Erro na consulta: {response.StatusCode}"
                );
            }

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ReceitaWsResponse>(content);

            if (data == null || data.Status == "ERROR")
            {
                return new CnpjLookupResult(
                    cleanCnpj,
                    string.Empty,
                    null,
                    "ERROR",
                    null,
                    null,
                    new List<string>(),
                    null, null, null, null, null, null, null, null, null,
                    false,
                    data?.Message ?? "Erro desconhecido"
                );
            }

            var cnaeSecundarios = data.AtividadesSecundarias?
                .Select(a => $"{a.Code} - {a.Text}")
                .ToList() ?? new List<string>();

            return new CnpjLookupResult(
                cleanCnpj,
                data.Nome ?? string.Empty,
                data.Fantasia,
                data.Situacao ?? "DESCONHECIDO",
                ParseDate(data.Abertura),
                data.AtividadePrincipal?.FirstOrDefault() != null 
                    ? $"{data.AtividadePrincipal[0].Code} - {data.AtividadePrincipal[0].Text}"
                    : null,
                cnaeSecundarios,
                data.Logradouro,
                data.Numero,
                data.Complemento,
                data.Bairro,
                data.Municipio,
                data.Uf,
                data.Cep,
                data.Telefone,
                data.Email,
                true
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar CNPJ");
            return new CnpjLookupResult(
                cnpj,
                string.Empty,
                null,
                "ERROR",
                null,
                null,
                new List<string>(),
                null, null, null, null, null, null, null, null, null,
                false,
                ex.Message
            );
        }
    }

    private static string NormalizeCnpj(string cnpj)
    {
        return new string(cnpj.Where(char.IsDigit).ToArray());
    }

    private static bool IsValidCnpj(string cnpj)
    {
        if (cnpj.Length != 14) return false;
        if (cnpj.All(c => c == cnpj[0])) return false; // Todos iguais
        return true; // Validação completa de dígitos verificadores pode ser adicionada
    }

    private static string MaskCnpj(string cnpj)
    {
        if (cnpj.Length == 14)
        {
            return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
        }
        return cnpj;
    }

    private static DateTime? ParseDate(string? dateStr)
    {
        if (string.IsNullOrEmpty(dateStr)) return null;
        
        // ReceitaWS retorna no formato dd/MM/yyyy
        if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
        {
            return date;
        }
        return null;
    }
}

#region ReceitaWS Response DTOs

internal class ReceitaWsResponse
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("cnpj")]
    public string? Cnpj { get; set; }

    [JsonPropertyName("nome")]
    public string? Nome { get; set; }

    [JsonPropertyName("fantasia")]
    public string? Fantasia { get; set; }

    [JsonPropertyName("abertura")]
    public string? Abertura { get; set; }

    [JsonPropertyName("situacao")]
    public string? Situacao { get; set; }

    [JsonPropertyName("tipo")]
    public string? Tipo { get; set; }

    [JsonPropertyName("porte")]
    public string? Porte { get; set; }

    [JsonPropertyName("natureza_juridica")]
    public string? NaturezaJuridica { get; set; }

    [JsonPropertyName("atividade_principal")]
    public List<AtividadeEconomica>? AtividadePrincipal { get; set; }

    [JsonPropertyName("atividades_secundarias")]
    public List<AtividadeEconomica>? AtividadesSecundarias { get; set; }

    [JsonPropertyName("logradouro")]
    public string? Logradouro { get; set; }

    [JsonPropertyName("numero")]
    public string? Numero { get; set; }

    [JsonPropertyName("complemento")]
    public string? Complemento { get; set; }

    [JsonPropertyName("bairro")]
    public string? Bairro { get; set; }

    [JsonPropertyName("municipio")]
    public string? Municipio { get; set; }

    [JsonPropertyName("uf")]
    public string? Uf { get; set; }

    [JsonPropertyName("cep")]
    public string? Cep { get; set; }

    [JsonPropertyName("telefone")]
    public string? Telefone { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

internal class AtividadeEconomica
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

#endregion
