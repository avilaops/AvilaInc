using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

[BsonIgnoreExtraElements]
public class Book : MongoEntity
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("author")]
    public string Author { get; set; } = string.Empty;

    [BsonElement("pdfUrl")]
    public string PdfUrl { get; set; } = string.Empty;

    [BsonElement("cover")]
    public string? Cover { get; set; }

    [BsonElement("totalPages")]
    public int TotalPages { get; set; }

    [BsonElement("currentPage")]
    public int CurrentPage { get; set; } = 0;

    [BsonElement("progress")]
    public decimal Progress { get; set; } = 0;

    [BsonElement("lastRead")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastRead { get; set; }

    [BsonElement("notes")]
    public List<string> Notes { get; set; } = new();

    [BsonElement("questions")]
    public List<string> Questions { get; set; } = new();
}
