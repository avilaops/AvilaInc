using Manager.Core.Common;

namespace Manager.Core.Entities.Identity;

public class User : BaseEntity
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; } = true;
    public bool EmailVerified { get; set; } = false;
    public DateTime? LastLoginAt { get; set; }
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }
    public string? VerifyToken { get; set; }
    public DateTime? VerifyTokenExpires { get; set; }
    
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}
