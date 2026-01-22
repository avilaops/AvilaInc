using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Manager.Integrations.Google;

public interface IGooglePlacesService
{
    Task<List<GooglePlaceDto>> SearchPlacesAsync(string query, double? lat, double? lng, int? radiusMeters, int maxResults);
    Task<GooglePlaceDto?> GetPlaceDetailsAsync(string placeId);
}

public class GooglePlacesService : IGooglePlacesService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GooglePlacesService> _logger;
    private readonly string _apiKey;
    private const string BaseUrl = "https://maps.googleapis.com/maps/api/place";

    public GooglePlacesService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GooglePlacesService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["Integrations:GoogleMaps:ApiKey"]
            ?? throw new ArgumentException("Google Maps API Key não configurada");
    }

    public async Task<List<GooglePlaceDto>> SearchPlacesAsync(
        string query,
        double? lat,
        double? lng,
        int? radiusMeters,
        int maxResults)
    {
        try
        {
            var url = $"{BaseUrl}/textsearch/json?query={Uri.EscapeDataString(query)}&key={_apiKey}";

            if (lat.HasValue && lng.HasValue)
            {
                url += $"&location={lat.Value},{lng.Value}";
                if (radiusMeters.HasValue)
                {
                    url += $"&radius={radiusMeters.Value}";
                }
            }

            _logger.LogInformation("Buscando lugares: {Query}", query);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GooglePlacesSearchResponse>(content);

            if (result?.Status != "OK" && result?.Status != "ZERO_RESULTS")
            {
                _logger.LogWarning("Google Places API retornou status: {Status}", result?.Status);
                return new List<GooglePlaceDto>();
            }

            var places = result.Results?
                .Take(maxResults)
                .Select(r => new GooglePlaceDto(
                    r.PlaceId,
                    r.Name,
                    r.FormattedAddress,
                    r.Geometry?.Location?.Lat,
                    r.Geometry?.Location?.Lng,
                    r.FormattedPhoneNumber,
                    r.Website,
                    r.Types ?? new List<string>(),
                    r.Rating,
                    r.UserRatingsTotal,
                    r.BusinessStatus
                ))
                .ToList() ?? new List<GooglePlaceDto>();

            _logger.LogInformation("Encontrados {Count} lugares", places.Count);
            return places;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar lugares no Google Places");
            throw;
        }
    }

    public async Task<GooglePlaceDto?> GetPlaceDetailsAsync(string placeId)
    {
        try
        {
            var url = $"{BaseUrl}/details/json?place_id={placeId}&key={_apiKey}&fields=place_id,name,formatted_address,geometry,formatted_phone_number,website,types,rating,user_ratings_total,business_status";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GooglePlaceDetailsResponse>(content);

            if (result?.Status != "OK")
            {
                _logger.LogWarning("Place details retornou status: {Status}", result?.Status);
                return null;
            }

            var place = result.Result;
            if (place == null) return null;

            return new GooglePlaceDto(
                place.PlaceId,
                place.Name,
                place.FormattedAddress,
                place.Geometry?.Location?.Lat,
                place.Geometry?.Location?.Lng,
                place.FormattedPhoneNumber,
                place.Website,
                place.Types ?? new List<string>(),
                place.Rating,
                place.UserRatingsTotal,
                place.BusinessStatus
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar detalhes do lugar {PlaceId}", placeId);
            throw;
        }
    }
}

#region DTOs

public record GooglePlaceDto(
    string PlaceId,
    string Name,
    string? FormattedAddress,
    double? Lat,
    double? Lng,
    string? Phone,
    string? Website,
    List<string> Types,
    double? Rating,
    int? UserRatingsTotal,
    string? BusinessStatus
);

internal class GooglePlacesSearchResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("results")]
    public List<PlaceResult> Results { get; set; } = new();
}

internal class GooglePlaceDetailsResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("result")]
    public PlaceResult? Result { get; set; }
}

internal class PlaceResult
{
    [JsonPropertyName("place_id")]
    public string PlaceId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("formatted_address")]
    public string? FormattedAddress { get; set; }

    [JsonPropertyName("geometry")]
    public Geometry? Geometry { get; set; }

    [JsonPropertyName("formatted_phone_number")]
    public string? FormattedPhoneNumber { get; set; }

    [JsonPropertyName("website")]
    public string? Website { get; set; }

    [JsonPropertyName("types")]
    public List<string>? Types { get; set; }

    [JsonPropertyName("rating")]
    public double? Rating { get; set; }

    [JsonPropertyName("user_ratings_total")]
    public int? UserRatingsTotal { get; set; }

    [JsonPropertyName("business_status")]
    public string? BusinessStatus { get; set; }
}

internal class Geometry
{
    [JsonPropertyName("location")]
    public LatLng? Location { get; set; }
}

internal class LatLng
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lng")]
    public double Lng { get; set; }
}

#endregion