namespace Manager.Contracts.DTOs;

public record GooglePlacesSearchRequest(
    string Query,
    double? Lat = null,
    double? Lng = null,
    int? RadiusMeters = null,
    int MaxResults = 20
);

public record GooglePlacesImportRequest(
    List<string> PlaceIds
);

public record GooglePlaceResult(
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

public record LocationDto(double Lat, double Lng);

public record CreateCompanyRequest(
    string Name,
    string? Cnpj = null,
    string? Address = null,
    string? City = null,
    string? State = null,
    string? ZipCode = null,
    string? Phone = null,
    string? Email = null,
    string? Website = null
);
