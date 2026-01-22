using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

[BsonIgnoreExtraElements]
public class ConversionFunnel : MongoEntity
{
    [BsonElement("funnelId")]
    public string FunnelId { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("siteId")]
    public string SiteId { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("steps")]
    public List<FunnelStep> Steps { get; set; } = new();

    [BsonElement("active")]
    public bool Active { get; set; } = true;
}

[BsonIgnoreExtraElements]
public class FunnelStep
{
    [BsonElement("stepNumber")]
    public int StepNumber { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("matchType")]
    public string MatchType { get; set; } = "url"; // url, event, or custom

    [BsonElement("matchValue")]
    public string MatchValue { get; set; } = string.Empty;

    [BsonElement("required")]
    public bool Required { get; set; } = true;
}

[BsonIgnoreExtraElements]
public class FunnelProgress : MongoEntity
{
    [BsonElement("funnelId")]
    public string FunnelId { get; set; } = string.Empty;

    [BsonElement("sessionId")]
    public string SessionId { get; set; } = string.Empty;

    [BsonElement("visitorId")]
    public string VisitorId { get; set; } = string.Empty;

    [BsonElement("siteId")]
    public string SiteId { get; set; } = string.Empty;

    [BsonElement("currentStep")]
    public int CurrentStep { get; set; } = 0;

    [BsonElement("completed")]
    public bool Completed { get; set; } = false;

    [BsonElement("completedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? CompletedAt { get; set; }

    [BsonElement("stepTimestamps")]
    public Dictionary<int, DateTime> StepTimestamps { get; set; } = new();
}
