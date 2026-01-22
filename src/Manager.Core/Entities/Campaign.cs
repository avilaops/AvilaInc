using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

[BsonIgnoreExtraElements]
public class Campaign : MongoEntity
{
    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;

    [BsonElement("tipo")]
    public string Tipo { get; set; } = "email";

    [BsonElement("status")]
    public string Status { get; set; } = "rascunho";

    [BsonElement("dataInicio")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DataInicio { get; set; }

    [BsonElement("dataFim")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DataFim { get; set; }

    [BsonElement("assunto")]
    public string? Assunto { get; set; }

    [BsonElement("mensagem")]
    public string? Mensagem { get; set; }

    [BsonElement("destinatarios")]
    public int Destinatarios { get; set; } = 0;

    [BsonElement("enviados")]
    public int Enviados { get; set; } = 0;

    [BsonElement("abertos")]
    public int Abertos { get; set; } = 0;

    [BsonElement("cliques")]
    public int Cliques { get; set; } = 0;

    [BsonElement("conversoes")]
    public int Conversoes { get; set; } = 0;

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();
}
