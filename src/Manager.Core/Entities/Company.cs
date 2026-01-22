using Manager.Core.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

public class Company : MongoEntity
{
    [BsonElement("tenantId")]
    public string? TenantId { get; set; }

    [BsonElement("source")]
    public string Source { get; set; } = string.Empty; // GooglePlaces, CNPJ, Manual

    [BsonElement("googlePlaceId")]
    public string? GooglePlaceId { get; set; }

    [BsonElement("cnpj")]
    public string? Cnpj { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("fantasyName")]
    public string? FantasyName { get; set; }

    [BsonElement("address")]
    public string? Address { get; set; }

    [BsonElement("city")]
    public string? City { get; set; }

    [BsonElement("state")]
    public string? State { get; set; }

    [BsonElement("zipCode")]
    public string? ZipCode { get; set; }

    [BsonElement("location")]
    public Location? Location { get; set; }

    [BsonElement("phone")]
    public string? Phone { get; set; }

    [BsonElement("email")]
    public string? Email { get; set; }

    [BsonElement("website")]
    public string? Website { get; set; }

    [BsonElement("types")]
    public List<string> Types { get; set; } = new();

    [BsonElement("rating")]
    public double? Rating { get; set; }

    [BsonElement("userRatingsTotal")]
    public int? UserRatingsTotal { get; set; }

    [BsonElement("businessStatus")]
    public string? BusinessStatus { get; set; }

    [BsonElement("cadastralStatus")]
    public string? CadastralStatus { get; set; }

    [BsonElement("openingDate")]
    public DateTime? OpeningDate { get; set; }

    [BsonElement("cnaeMain")]
    public string? CnaeMain { get; set; }

    [BsonElement("cnaeSecondary")]
    public List<string> CnaeSecondary { get; set; } = new();

    [BsonElement("lastSyncedAt")]
    public DateTime? LastSyncedAt { get; set; }

    [BsonElement("lastCnpjLookupAt")]
    public DateTime? LastCnpjLookupAt { get; set; }

    [BsonElement("rawJson")]
    public string? RawJson { get; set; }
}

public class Location
{
    [BsonElement("lat")]
    public double Lat { get; set; }

    [BsonElement("lng")]
    public double Lng { get; set; }
}
