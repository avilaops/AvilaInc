using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

[BsonIgnoreExtraElements]
public class Email : MongoEntity
{
    [BsonElement("account")]
    public string Account { get; set; } = string.Empty;

    [BsonElement("messageId")]
    public string MessageId { get; set; } = string.Empty;

    [BsonElement("threadId")]
    public string ThreadId { get; set; } = string.Empty;

    [BsonElement("from")]
    public string From { get; set; } = string.Empty;

    [BsonElement("to")]
    public List<string> To { get; set; } = new();

    [BsonElement("subject")]
    public string Subject { get; set; } = string.Empty;

    [BsonElement("snippet")]
    public string? Snippet { get; set; }

    [BsonElement("body")]
    public string? Body { get; set; }

    [BsonElement("labels")]
    public List<string> Labels { get; set; } = new();

    [BsonElement("isRead")]
    public bool IsRead { get; set; } = false;

    [BsonElement("isStarred")]
    public bool IsStarred { get; set; } = false;

    [BsonElement("date")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Date { get; set; }

    [BsonElement("hasAttachments")]
    public bool HasAttachments { get; set; } = false;

    [BsonElement("syncedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}
