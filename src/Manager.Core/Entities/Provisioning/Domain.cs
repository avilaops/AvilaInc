using Manager.Core.Common;

namespace Manager.Core.Entities.Provisioning;

public class Domain : BaseEntity
{
    public required Guid ProjectId { get; set; }
    public required string DomainName { get; set; }
    public string? ZoneId { get; set; }
    public bool IsVerified { get; set; } = false;
    public DateTime? VerifiedAt { get; set; }
    public string? DnsRecords { get; set; } // JSON
}
