using Manager.Core.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

public sealed class WebsiteRequest : MongoEntity
{
    [BsonElement("businessName")]
    public string BusinessName { get; set; } = string.Empty;

    [BsonElement("niche")]
    public string Niche { get; set; } = string.Empty;

    [BsonElement("city")]
    public string City { get; set; } = string.Empty;

    [BsonElement("services")]
    public List<string> Services { get; set; } = new();

    [BsonElement("differentials")]
    public string Differentials { get; set; } = string.Empty;

    [BsonElement("whatsapp")]
    public string WhatsApp { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("templateType")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public WebsiteTemplateType TemplateType { get; set; }

    [BsonElement("colorPreference")]
    public string? ColorPreference { get; set; }

    [BsonElement("logoUrl")]
    public string? LogoUrl { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public WebsiteRequestStatus Status { get; set; } = WebsiteRequestStatus.Received;

    [BsonElement("projectId")]
    public string? ProjectId { get; set; }

    [BsonElement("errorMessage")]
    public string? ErrorMessage { get; set; }

    [BsonElement("customerIp")]
    public string? CustomerIp { get; set; }

    [BsonElement("customerUserAgent")]
    public string? CustomerUserAgent { get; set; }
}
