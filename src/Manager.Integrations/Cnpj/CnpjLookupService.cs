using Manager.Contracts.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Manager.Integrations.Cnpj;

public interface ICnpjLookupService
{
    Task<CnpjLookupResultDto> LookupAsync(string cnpj);
}

public class CnpjLookupService : ICnpjLookupService
{
    private readonly ICnpjLookupProvider _provider;
    private readonly ILogger<CnpjLookupService> _logger;

    public CnpjLookupService(ICnpjLookupProvider provider, ILogger<CnpjLookupService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task<CnpjLookupResultDto> LookupAsync(string cnpj)
    {
        // Normalize CNPJ (remove formatting)
        var normalizedCnpj = NormalizeCnpj(cnpj);

        if (!IsValidCnpj(normalizedCnpj))
        {
            throw new ArgumentException("CNPJ inválido", nameof(cnpj));
        }

        try
        {
            _logger.LogInformation("Consultando CNPJ: {Cnpj}", MaskCnpj(normalizedCnpj));
            var result = await _provider.LookupAsync(normalizedCnpj);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar CNPJ: {Cnpj}", MaskCnpj(normalizedCnpj));
            throw;
        }
    }

    private static string NormalizeCnpj(string cnpj)
    {
        return new string(cnpj.Where(char.IsDigit).ToArray());
    }

    private static bool IsValidCnpj(string cnpj)
    {
        if (cnpj.Length != 14)
            return false;

        // Basic CNPJ validation algorithm
        var multipliers1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multipliers2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var digits = cnpj.Select(c => int.Parse(c.ToString())).ToArray();

        // Check if all digits are the same
        if (digits.Distinct().Count() == 1)
            return false;

        // First verification digit
        var sum1 = digits.Take(12).Select((d, i) => d * multipliers1[i]).Sum();
        var remainder1 = sum1 % 11;
        var digit1 = remainder1 < 2 ? 0 : 11 - remainder1;

        if (digit1 != digits[12])
            return false;

        // Second verification digit
        var sum2 = digits.Take(13).Select((d, i) => d * multipliers2[i]).Sum();
        var remainder2 = sum2 % 11;
        var digit2 = remainder2 < 2 ? 0 : 11 - remainder2;

        return digit2 == digits[13];
    }

    private static string MaskCnpj(string cnpj)
    {
        if (cnpj.Length != 14) return cnpj;
        return $"{cnpj[..2]}.{cnpj[2..5]}.{cnpj[5..8]}/{cnpj[8..12]}-{cnpj[12..]}";
    }
}

// Example implementation using a configurable HTTP provider
public class HttpCnpjLookupProvider : ICnpjLookupProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string? _apiKey;
    private readonly ILogger<HttpCnpjLookupProvider> _logger;

    public HttpCnpjLookupProvider(HttpClient httpClient, IConfiguration configuration, ILogger<HttpCnpjLookupProvider> logger)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["CnpjLookup:BaseUrl"] ?? throw new ArgumentNullException("CnpjLookup:BaseUrl not configured");
        _apiKey = configuration["CnpjLookup:ApiKey"];
        _logger = logger;

        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);

        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }
    }

    public async Task<CnpjLookupResultDto> LookupAsync(string cnpj)
    {
        try
        {
            var url = $"cnpj/{cnpj}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            // Parse the response according to the provider's format
            // This is a placeholder - adjust based on actual API response format
            var result = JsonSerializer.Deserialize<CnpjLookupResultDto>(content);

            if (result == null)
            {
                throw new InvalidOperationException("Invalid response from CNPJ provider");
            }

            result = result with { RawJson = content };
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling CNPJ provider for: {Cnpj}", cnpj);
            throw;
        }
    }
}