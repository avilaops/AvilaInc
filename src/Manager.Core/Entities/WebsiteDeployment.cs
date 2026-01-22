using Manager.Core.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

public sealed class WebsiteDeployment : MongoEntity
{
    [BsonElement("projectId")]
    public string ProjectId { get; set; } = string.Empty;

    [BsonElement("provider")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public DeployProvider Provider { get; set; }

    [BsonElement("deployUrl")]
    public string DeployUrl { get; set; } = string.Empty;

    [BsonElement("commitHash")]
    public string? CommitHash { get; set; }

    [BsonElement("buildTime")]
    public TimeSpan? BuildTime { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public JobStatus Status { get; set; } = JobStatus.Pending;

    [BsonElement("logs")]
    public List<string> Logs { get; set; } = new();

    [BsonElement("errorMessage")]
    public string? ErrorMessage { get; set; }
}
