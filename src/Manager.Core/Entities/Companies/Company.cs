using Manager.Core.Common;
using NetTopologySuite.Geometries;

namespace Manager.Core.Entities.Companies;

public class Company : BaseEntity
{
    public required string Source { get; set; } // "GooglePlaces", "Manual", etc.
    public required string Name { get; set; }
    public string? GooglePlaceId { get; set; } // Unique identifier from Google Places
    public string? Address { get; set; }
    public Point? Location { get; set; } // Lat/Lng using NetTopologySuite
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Types { get; set; } // JSON array of place types
    public double? Rating { get; set; }
    public int? UserRatingsTotal { get; set; }
    public string? BusinessStatus { get; set; } // "OPERATIONAL", "CLOSED_TEMPORARILY", etc.
    public DateTime? LastSyncedAt { get; set; }
    public string? RawJson { get; set; } // Store full Google Places response for debugging
    public string? Cnpj { get; set; } // Brazilian company identifier
    public DateTime? LastCnpjLookupAt { get; set; }

    // Navigation properties if needed
    // public Guid? TenantId { get; set; }
}