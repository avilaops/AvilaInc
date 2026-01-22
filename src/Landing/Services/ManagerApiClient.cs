using System.Net.Http.Json;
using Manager.Contracts.DTOs;
using Manager.Core.Enums;

namespace Landing.Services;

/// <summary>
/// Cliente tipado para comunicação com Manager.Api
/// </summary>
public sealed class ManagerApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ManagerApiClient> _logger;

    public ManagerApiClient(HttpClient httpClient, ILogger<ManagerApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Envia lead capturado na landing para o backend
    /// </summary>
    public async Task<ApiResponse<string>> CreateLeadAsync(
        CreateLeadDto dto, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/public/leads", 
                dto, 
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CreateLeadResponse>(
                    cancellationToken: cancellationToken);
                
                _logger.LogInformation("Lead criado com sucesso: {Email}", dto.Email);
                return ApiResponse<string>.CreateSuccess(result?.Id ?? "ok");
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning(
                "Falha ao criar lead. Status: {StatusCode}, Body: {Body}",
                response.StatusCode,
                errorContent);

            return ApiResponse<string>.CreateFailure("Não foi possível enviar seus dados no momento.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de rede ao criar lead");
            return ApiResponse<string>.CreateFailure("Erro de conexão com o servidor. Tente novamente.");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout ao criar lead");
            return ApiResponse<string>.CreateFailure("Tempo esgotado. Verifique sua conexão e tente novamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar lead");
            return ApiResponse<string>.CreateFailure("Erro inesperado. Tente novamente mais tarde.");
        }
    }

    /// <summary>
    /// Cria um pedido de geração de website
    /// </summary>
    public async Task<WebsiteRequestResponseDto?> CreateWebsiteRequestAsync(
        CreateWebsiteRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/public/website-requests",
                dto,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<WebsiteRequestResponseDto>(
                    cancellationToken: cancellationToken);
            }

            _logger.LogWarning("Falha ao criar pedido de website. Status: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido de website");
            throw;
        }
    }

    /// <summary>
    /// Consulta o status de um pedido de website
    /// </summary>
    public async Task<WebsiteRequestResponseDto?> GetWebsiteRequestStatusAsync(
        string requestId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<WebsiteRequestResponseDto>(
                $"api/public/website-requests/{requestId}",
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar status do pedido {RequestId}", requestId);
            throw;
        }
    }

    /// <summary>
    /// Obtém o preview do website gerado
    /// </summary>
    public async Task<WebsiteProjectDto?> GetWebsitePreviewAsync(
        string requestId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<WebsiteProjectDto>(
                $"api/public/website-requests/{requestId}/preview",
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter preview do pedido {RequestId}", requestId);
            throw;
        }
    }

    /// <summary>
    /// Publica o website (torna público)
    /// </summary>
    public async Task<bool> PublishWebsiteAsync(
        string requestId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                $"api/public/website-requests/{requestId}/publish",
                null,
                cancellationToken);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar website {RequestId}", requestId);
            throw;
        }
    }
}

/// <summary>
/// DTO para envio de lead ao backend
/// </summary>
public sealed record CreateLeadDto(
    string Name,
    string Email,
    string Phone,
    string? Message,
    string Source
);

/// <summary>
/// Resposta padrão da API
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    public static ApiResponse<T> CreateSuccess(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static ApiResponse<T> CreateFailure(string errorMessage) => new()
    {
        Success = false,
        ErrorMessage = errorMessage
    };
}

/// <summary>
/// Resposta do endpoint de criação de lead
/// </summary>
internal sealed class CreateLeadResponse
{
    public bool Ok { get; set; }
    public string? Id { get; set; }
}
