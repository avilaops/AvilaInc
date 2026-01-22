using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

[BsonIgnoreExtraElements]
public class Gestor : MongoEntity
{
    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("telefone")]
    public string? Telefone { get; set; }

    [BsonElement("cargo")]
    public string Cargo { get; set; } = string.Empty;

    [BsonElement("departamento")]
    public string? Departamento { get; set; }

    [BsonElement("ativo")]
    public bool Ativo { get; set; } = true;

    [BsonElement("permissoes")]
    public List<string> Permissoes { get; set; } = new();

    [BsonElement("ultimoLogin")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UltimoLogin { get; set; }
}
