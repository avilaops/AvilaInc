using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

[BsonIgnoreExtraElements]
public class HeatmapClick : MongoEntity
{
    [BsonElement("siteId")]
    public string SiteId { get; set; } = string.Empty;

    [BsonElement("sessionId")]
    public string SessionId { get; set; } = string.Empty;

    [BsonElement("visitorId")]
    public string VisitorId { get; set; } = string.Empty;

    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;

    [BsonElement("path")]
    public string Path { get; set; } = string.Empty;

    [BsonElement("x")]
    public int X { get; set; }

    [BsonElement("y")]
    public int Y { get; set; }

    [BsonElement("screenWidth")]
    public int ScreenWidth { get; set; }

    [BsonElement("screenHeight")]
    public int ScreenHeight { get; set; }

    [BsonElement("elementTag")]
    public string? ElementTag { get; set; }

    [BsonElement("elementId")]
    public string? ElementId { get; set; }

    [BsonElement("elementClass")]
    public string? ElementClass { get; set; }

    [BsonElement("timestamp")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

[BsonIgnoreExtraElements]
public class HeatmapScroll : MongoEntity
{
    [BsonElement("siteId")]
    public string SiteId { get; set; } = string.Empty;

    [BsonElement("sessionId")]
    public string SessionId { get; set; } = string.Empty;

    [BsonElement("visitorId")]
    public string VisitorId { get; set; } = string.Empty;

    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;

    [BsonElement("path")]
    public string Path { get; set; } = string.Empty;

    [BsonElement("maxDepth")]
    public int MaxDepth { get; set; } // percentage 0-100

    [BsonElement("timestamp")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

[BsonIgnoreExtraElements]
public class HeatmapMove : MongoEntity
{
    [BsonElement("siteId")]
    public string SiteId { get; set; } = string.Empty;

    [BsonElement("sessionId")]
    public string SessionId { get; set; } = string.Empty;

    [BsonElement("visitorId")]
    public string VisitorId { get; set; } = string.Empty;

    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;

    [BsonElement("path")]
    public string Path { get; set; } = string.Empty;

    [BsonElement("positions")]
    public List<MousePosition> Positions { get; set; } = new();

    [BsonElement("timestamp")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class MousePosition
{
    [BsonElement("x")]
    public int X { get; set; }

    [BsonElement("y")]
    public int Y { get; set; }

    [BsonElement("time")]
    public int Time { get; set; } // milliseconds since page load
}
