using Manager.Core.Common;

namespace Manager.Core.Entities.Identity;

public class Session : BaseEntity
{
    public required Guid UserId { get; set; }
    public required string Token { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsRevoked { get; set; } = false;
    
    public User? User { get; set; }
}
