using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Manager.Contracts.DTOs;
using Manager.Core.Entities;
using Manager.Infrastructure.Repositories;
using Manager.Integrations.Google;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/google-places")]
[Authorize]
public class GooglePlacesController : ControllerBase
{
    private readonly IGooglePlacesService _googlePlaces;
    private readonly ICompanyMongoRepository _companyRepo;
    private readonly ILogger<GooglePlacesController> _logger;

    public GooglePlacesController(
        IGooglePlacesService googlePlaces,
        ICompanyMongoRepository companyRepo,
        ILogger<GooglePlacesController> logger)
    {
        _googlePlaces = googlePlaces;
        _companyRepo = companyRepo;
        _logger = logger;
    }

    /// <summary>
    /// Buscar empresas no Google Places
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] GooglePlacesSearchRequest request)
    {
        try
        {
            var places = await _googlePlaces.SearchPlacesAsync(
                request.Query,
                request.Lat,
                request.Lng,
                request.RadiusMeters,
                request.MaxResults
            );

            var results = places.Select(p => new GooglePlaceResult(
                p.PlaceId,
                p.Name,
                p.FormattedAddress,
                p.Lat,
                p.Lng,
                p.Phone,
                p.Website,
                p.Types,
                p.Rating,
                p.UserRatingsTotal,
                p.BusinessStatus
            )).ToList();

            return Ok(new { success = true, count = results.Count, results });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar no Google Places");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Importar empresas do Google Places para o banco
    /// </summary>
    [HttpPost("import")]
    public async Task<IActionResult> Import([FromBody] GooglePlacesImportRequest request)
    {
        try
        {
            var imported = new List<CompanyDto>();
            var errors = new List<string>();

            foreach (var placeId in request.PlaceIds)
            {
                try
                {
                    var place = await _googlePlaces.GetPlaceDetailsAsync(placeId);
                    if (place == null)
                    {
                        errors.Add($"Place {placeId} não encontrado");
                        continue;
                    }

                    var company = new Company
                    {
                        Id = null, // MongoDB irá gerar
                        Source = "GooglePlaces",
                        GooglePlaceId = place.PlaceId,
                        Name = place.Name,
                        Address = place.FormattedAddress,
                        Location = place.Lat.HasValue && place.Lng.HasValue
                            ? new Location { Lat = place.Lat.Value, Lng = place.Lng.Value }
                            : null,
                        Phone = place.Phone,
                        Website = place.Website,
                        Types = place.Types,
                        Rating = place.Rating,
                        UserRatingsTotal = place.UserRatingsTotal,
                        BusinessStatus = place.BusinessStatus,
                        LastSyncedAt = DateTime.UtcNow
                    };

                    var saved = await _companyRepo.UpsertByGooglePlaceIdAsync(company);
                    imported.Add(MapToDto(saved));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao importar place {PlaceId}", placeId);
                    errors.Add($"{placeId}: {ex.Message}");
                }
            }

            return Ok(new
            {
                success = true,
                imported = imported.Count,
                errors = errors.Count,
                companies = imported,
                errorMessages = errors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao importar empresas");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    private static CompanyDto MapToDto(Company c) => new()
    {
        Id = c.Id,
        Source = c.Source,
        GooglePlaceId = c.GooglePlaceId,
        Cnpj = c.Cnpj,
        Name = c.Name,
        FantasyName = c.FantasyName,
        Address = c.Address,
        City = c.City,
        State = c.State,
        ZipCode = c.ZipCode,
        Latitude = c.Location?.Lat,
        Longitude = c.Location?.Lng,
        Phone = c.Phone,
        Email = c.Email,
        Website = c.Website,
        Types = System.Text.Json.JsonSerializer.Serialize(c.Types),
        Rating = c.Rating,
        UserRatingsTotal = c.UserRatingsTotal,
        BusinessStatus = c.BusinessStatus,
        CadastralStatus = c.CadastralStatus,
        OpeningDate = c.OpeningDate,
        CnaeMain = c.CnaeMain,
        CnaeSecondary = c.CnaeSecondary,
        LastSyncedAt = c.LastSyncedAt,
        LastCnpjLookupAt = c.LastCnpjLookupAt,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}