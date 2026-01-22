using Manager.Core.Common;

namespace Manager.Core.Entities.Provisioning;

public class Secret : BaseEntity
{
    public required Guid ProviderAccountId { get; set; }
    public required string Key { get; set; }
    public required string EncryptedValue { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ProviderAccount? ProviderAccount { get; set; }
}
