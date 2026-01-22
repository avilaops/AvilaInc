using Manager.Core.Common;
using Manager.Core.Enums;

namespace Manager.Core.Entities.Deploy;

public class Deployment : BaseEntity
{
    public required Guid ProjectId { get; set; }
    public required string Version { get; set; }
    public required DeployProvider Provider { get; set; }
    public string? Environment { get; set; } = "production";
    public string? CommitHash { get; set; }
    public string? DeploymentUrl { get; set; }
    public string? Logs { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsSuccessful { get; set; } = false;
}
