using Manager.Core.Common;

namespace Manager.Core.Entities.Orchestration;

public class Playbook : BaseEntity
{
    public required string Name { get; set; }
    public required string Code { get; set; } // Landing, Website, etc.
    public string? Description { get; set; }
    public required string StepsJson { get; set; } // JSON array of steps
    public bool IsActive { get; set; } = true;
    public int Version { get; set; } = 1;
    
    public ICollection<PlaybookRun> Runs { get; set; } = new List<PlaybookRun>();
}
