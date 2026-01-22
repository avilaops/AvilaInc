using Manager.Core.Common;
using Manager.Core.Enums;

namespace Manager.Core.Entities.Orchestration;

public class StepRun : BaseEntity
{
    public required Guid PlaybookRunId { get; set; }
    public required string StepName { get; set; }
    public required int StepOrder { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Queued;
    public string? InputJson { get; set; }
    public string? OutputJson { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; } = 0;
    
    public PlaybookRun? PlaybookRun { get; set; }
}
