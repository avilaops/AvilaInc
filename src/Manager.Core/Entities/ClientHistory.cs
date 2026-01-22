using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

[BsonIgnoreExtraElements]
public class ClientHistory : MongoEntity
{
    [BsonElement("clientId")]
    public string ClientId { get; set; } = string.Empty;

    [BsonElement("action")]
    public string Action { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("performedBy")]
    public string? PerformedBy { get; set; }

    [BsonElement("metadata")]
    public Dictionary<string, string> Metadata { get; set; } = new();
}
