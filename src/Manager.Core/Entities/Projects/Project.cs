using Manager.Core.Common;
using Manager.Core.Enums;
using Manager.Core.Entities.Deploy;

namespace Manager.Core.Entities.Projects;

public class Project : BaseEntity
{
    public required string Name { get; set; }
    public required Guid ClientId { get; set; }
    public required ProjectType Type { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Draft;
    public string? Description { get; set; }
    public string? Domain { get; set; }
    public string? RepositoryUrl { get; set; }
    public string? DeploymentUrl { get; set; }
    public DateTime? LaunchedAt { get; set; }
    
    public ProjectSpec? Spec { get; set; }
    public ICollection<Deployment> Deployments { get; set; } = new List<Deployment>();
}
