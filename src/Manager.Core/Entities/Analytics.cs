using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

[BsonIgnoreExtraElements]
public class Visitor : MongoEntity
{
    [BsonElement("visitorId")]
    public string VisitorId { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("fingerprint")]
    public string? Fingerprint { get; set; }

    [BsonElement("siteId")]
    public string SiteId { get; set; } = string.Empty;

    [BsonElement("firstSeen")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime FirstSeen { get; set; } = DateTime.UtcNow;

    [BsonElement("lastSeen")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime LastSeen { get; set; } = DateTime.UtcNow;

    [BsonElement("totalPageviews")]
    public int TotalPageviews { get; set; }

    [BsonElement("totalSessions")]
    public int TotalSessions { get; set; }

    [BsonElement("country")]
    public string? Country { get; set; }

    [BsonElement("city")]
    public string? City { get; set; }

    [BsonElement("browser")]
    public string? Browser { get; set; }

    [BsonElement("os")]
    public string? OS { get; set; }

    [BsonElement("device")]
    public string? Device { get; set; }

    [BsonElement("referrer")]
    public string? Referrer { get; set; }

    [BsonElement("utmSource")]
    public string? UtmSource { get; set; }

    [BsonElement("utmMedium")]
    public string? UtmMedium { get; set; }

    [BsonElement("utmCampaign")]
    public string? UtmCampaign { get; set; }
}

[BsonIgnoreExtraElements]
public class Session : MongoEntity
{
    [BsonElement("sessionId")]
    public string SessionId { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("visitorId")]
    public string VisitorId { get; set; } = string.Empty;

    [BsonElement("siteId")]
    public string SiteId { get; set; } = string.Empty;

    [BsonElement("startTime")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    [BsonElement("endTime")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? EndTime { get; set; }

    [BsonElement("duration")]
    public int Duration { get; set; } // seconds

    [BsonElement("pageviews")]
    public int Pageviews { get; set; }

    [BsonElement("events")]
    public int Events { get; set; }

    [BsonElement("bounced")]
    public bool Bounced { get; set; }

    [BsonElement("converted")]
    public bool Converted { get; set; }

    [BsonElement("landingPage")]
    public string? LandingPage { get; set; }

    [BsonElement("exitPage")]
    public string? ExitPage { get; set; }
}

[BsonIgnoreExtraElements]
public class PageView : MongoEntity
{
    [BsonElement("sessionId")]
    public string SessionId { get; set; } = string.Empty;

    [BsonElement("visitorId")]
    public string VisitorId { get; set; } = string.Empty;

    [BsonElement("siteId")]
    public string SiteId { get; set; } = string.Empty;

    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;

    [BsonElement("path")]
    public string Path { get; set; } = string.Empty;

    [BsonElement("title")]
    public string? Title { get; set; }

    [BsonElement("referrer")]
    public string? Referrer { get; set; }

    [BsonElement("timestamp")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [BsonElement("duration")]
    public int Duration { get; set; } // seconds

    [BsonElement("screenWidth")]
    public int? ScreenWidth { get; set; }

    [BsonElement("screenHeight")]
    public int? ScreenHeight { get; set; }

    [BsonElement("scrollDepth")]
    public int? ScrollDepth { get; set; } // percentage
}

[BsonIgnoreExtraElements]
public class AnalyticsEvent : MongoEntity
{
    [BsonElement("sessionId")]
    public string SessionId { get; set; } = string.Empty;

    [BsonElement("visitorId")]
    public string VisitorId { get; set; } = string.Empty;

    [BsonElement("siteId")]
    public string SiteId { get; set; } = string.Empty;

    [BsonElement("eventName")]
    public string EventName { get; set; } = string.Empty;

    [BsonElement("eventCategory")]
    public string? EventCategory { get; set; }

    [BsonElement("eventLabel")]
    public string? EventLabel { get; set; }

    [BsonElement("eventValue")]
    public decimal? EventValue { get; set; }

    [BsonElement("timestamp")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [BsonElement("url")]
    public string? Url { get; set; }

    [BsonElement("metadata")]
    public Dictionary<string, string>? Metadata { get; set; }
}

[BsonIgnoreExtraElements]
public class Site : MongoEntity
{
    [BsonElement("siteId")]
    public string SiteId { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("domain")]
    public string Domain { get; set; } = string.Empty;

    [BsonElement("trackingCode")]
    public string TrackingCode { get; set; } = string.Empty;

    [BsonElement("active")]
    public bool Active { get; set; } = true;

    [BsonElement("clientId")]
    public string? ClientId { get; set; }

    [BsonElement("allowedDomains")]
    public List<string> AllowedDomains { get; set; } = new();

    [BsonElement("chatEnabled")]
    public bool ChatEnabled { get; set; } = false;

    [BsonElement("chatWidget")]
    public ChatWidget? ChatWidget { get; set; }
}

[BsonIgnoreExtraElements]
public class ChatWidget
{
    [BsonElement("position")]
    public string Position { get; set; } = "bottom-right";

    [BsonElement("color")]
    public string Color { get; set; } = "#1976d2";

    [BsonElement("welcomeMessage")]
    public string WelcomeMessage { get; set; } = "Olá! Como posso ajudar?";

    [BsonElement("offlineMessage")]
    public string OfflineMessage { get; set; } = "Estamos offline. Deixe sua mensagem!";
}

[BsonIgnoreExtraElements]
public class ChatMessage : MongoEntity
{
    [BsonElement("chatId")]
    public string ChatId { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("siteId")]
    public string SiteId { get; set; } = string.Empty;

    [BsonElement("visitorId")]
    public string VisitorId { get; set; } = string.Empty;

    [BsonElement("visitorName")]
    public string? VisitorName { get; set; }

    [BsonElement("visitorEmail")]
    public string? VisitorEmail { get; set; }

    [BsonElement("message")]
    public string Message { get; set; } = string.Empty;

    [BsonElement("sender")]
    public string Sender { get; set; } = "visitor"; // visitor or agent

    [BsonElement("agentId")]
    public string? AgentId { get; set; }

    [BsonElement("timestamp")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [BsonElement("read")]
    public bool Read { get; set; } = false;
}
