using Manager.Core.Common;
using Manager.Core.Enums;

namespace Manager.Core.Entities.Orchestration;

public class PlaybookRun : BaseEntity
{
    public required Guid PlaybookId { get; set; }
    public required Guid ProjectId { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Queued;
    public string? InputJson { get; set; }
    public string? OutputJson { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    
    public Playbook? Playbook { get; set; }
    public ICollection<StepRun> StepRuns { get; set; } = new List<StepRun>();
}
