using Manager.Core.Common;

namespace Manager.Core.Entities.Audit;

public class AuditEvent : BaseEntity
{
    public required string EntityType { get; set; }
    public required Guid EntityId { get; set; }
    public required string Action { get; set; } // Created, Updated, Deleted, StatusChanged, etc.
    public string? UserId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Metadata { get; set; } // JSON
}
