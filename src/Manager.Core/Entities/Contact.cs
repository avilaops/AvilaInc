using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

[BsonIgnoreExtraElements]
public class Contact : MongoEntity
{
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string? Phone { get; set; }

    [BsonElement("company")]
    public string? Company { get; set; }

    [BsonElement("position")]
    public string? Position { get; set; }

    [BsonElement("source")]
    public string Source { get; set; } = "manual";

    [BsonElement("status")]
    public string Status { get; set; } = "active";

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();

    [BsonElement("notes")]
    public string? Notes { get; set; }

    [BsonElement("linkedinUrl")]
    public string? LinkedInUrl { get; set; }

    [BsonElement("lastContactDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastContactDate { get; set; }

    [BsonElement("importDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? ImportDate { get; set; }
}
