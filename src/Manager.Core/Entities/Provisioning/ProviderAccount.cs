using Manager.Core.Common;

namespace Manager.Core.Entities.Provisioning;

public class ProviderAccount : BaseEntity
{
    public required string Provider { get; set; } // GitHub, Cloudflare, etc.
    public required string AccountName { get; set; }
    public string? AccountId { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<Secret> Secrets { get; set; } = new List<Secret>();
}
