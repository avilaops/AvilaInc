using System.Net.Http.Json;
using System.Text.Json;

namespace Manager.Web.Services;

public interface IApiService
{
    Task<ApiResponse<T>> GetAsync<T>(string endpoint);
    Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data);
    Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data);
    Task<ApiResponse<bool>> DeleteAsync(string endpoint);
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
                return new ApiResponse<T> { Success = true, Data = data };
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("GET {Endpoint} failed: {Error}", endpoint, error);
            return new ApiResponse<T> { Success = false, Error = error };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling GET {Endpoint}", endpoint);
            return new ApiResponse<T> { Success = false, Error = ex.Message };
        }
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonOptions);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
                return new ApiResponse<T> { Success = true, Data = result };
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("POST {Endpoint} failed: {Error}", endpoint, error);
            return new ApiResponse<T> { Success = false, Error = error };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling POST {Endpoint}", endpoint);
            return new ApiResponse<T> { Success = false, Error = ex.Message };
        }
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data, _jsonOptions);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
                return new ApiResponse<T> { Success = true, Data = result };
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("PUT {Endpoint} failed: {Error}", endpoint, error);
            return new ApiResponse<T> { Success = false, Error = error };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling PUT {Endpoint}", endpoint);
            return new ApiResponse<T> { Success = false, Error = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool> { Success = true, Data = true };
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("DELETE {Endpoint} failed: {Error}", endpoint, error);
            return new ApiResponse<bool> { Success = false, Error = error };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling DELETE {Endpoint}", endpoint);
            return new ApiResponse<bool> { Success = false, Error = ex.Message };
        }
    }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
}
