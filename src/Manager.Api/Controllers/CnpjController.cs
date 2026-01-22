using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Manager.Integrations.Cnpj;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/cnpj")]
[Authorize]
public class CnpjController : ControllerBase
{
    private readonly ICnpjLookupProvider _cnpjProvider;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CnpjController> _logger;
    private static readonly SemaphoreSlim _rateLimiter = new(1, 1); // 1 req por vez

    public CnpjController(
        ICnpjLookupProvider cnpjProvider,
        IMemoryCache cache,
        ILogger<CnpjController> logger)
    {
        _cnpjProvider = cnpjProvider;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Buscar dados cadastrais por CNPJ
    /// </summary>
    [HttpGet("{cnpj}")]
    public async Task<IActionResult> Lookup(string cnpj)
    {
        try
        {
            var cleanCnpj = NormalizeCnpj(cnpj);
            
            // Verifica cache (10 minutos)
            var cacheKey = $"cnpj_{cleanCnpj}";
            if (_cache.TryGetValue<CnpjLookupResult>(cacheKey, out var cached))
            {
                _logger.LogInformation("CNPJ {Cnpj} retornado do cache", MaskCnpj(cleanCnpj));
                return Ok(new { success = true, cached = true, data = MapToDto(cached!) });
            }

            // Rate limit: 1 req/seg
            await _rateLimiter.WaitAsync();
            try
            {
                var result = await _cnpjProvider.LookupAsync(cleanCnpj);

                if (result.Success)
                {
                    // Cache por 10 minutos
                    _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
                }

                return Ok(new { success = result.Success, cached = false, data = MapToDto(result) });
            }
            finally
            {
                await Task.Delay(1000); // Rate limit: 1/seg
                _rateLimiter.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar CNPJ");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    private static string NormalizeCnpj(string cnpj)
    {
        return new string(cnpj.Where(char.IsDigit).ToArray());
    }

    private static string MaskCnpj(string cnpj)
    {
        if (cnpj.Length == 14)
        {
            return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
        }
        return cnpj;
    }

    private static Manager.Contracts.DTOs.CnpjLookupResult MapToDto(CnpjLookupResult r) => new(
        r.Cnpj,
        r.RazaoSocial,
        r.NomeFantasia,
        r.Status,
        r.DataAbertura,
        r.CnaeMain,
        r.CnaeSecondary,
        r.Logradouro,
        r.Numero,
        r.Complemento,
        r.Bairro,
        r.Municipio,
        r.Uf,
        r.Cep,
        r.Telefone,
        r.Email,
        r.Success,
        r.ErrorMessage
    );
}

            var result = await _cnpjLookupService.LookupAsync(normalizedCnpj);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid CNPJ format: {Cnpj}", cnpj);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error looking up CNPJ: {Cnpj}", cnpj);
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}