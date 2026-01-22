using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

[BsonIgnoreExtraElements]
public class Lead : MongoEntity
{
    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("telefone")]
    public string? Telefone { get; set; }

    [BsonElement("empresa")]
    public string? Empresa { get; set; }

    [BsonElement("cnpj")]
    public string? Cnpj { get; set; }

    [BsonElement("website")]
    public string? Website { get; set; }

    [BsonElement("origem")]
    public string Origem { get; set; } = "site";

    [BsonElement("status")]
    public string Status { get; set; } = "novo";

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();

    [BsonElement("observacoes")]
    public string? Observacoes { get; set; }

    [BsonElement("valorEstimado")]
    public decimal? ValorEstimado { get; set; }

    [BsonElement("probabilidade")]
    public int? Probabilidade { get; set; }

    [BsonElement("dataContato")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DataContato { get; set; }

    [BsonElement("dataConversao")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DataConversao { get; set; }

    [BsonElement("responsavel")]
    public string? Responsavel { get; set; }

    [BsonElement("campanha")]
    public string? Campanha { get; set; }

    [BsonElement("patentVerified")]
    public bool PatentVerified { get; set; } = false;

    [BsonElement("hasPatent")]
    public bool? HasPatent { get; set; }
}
