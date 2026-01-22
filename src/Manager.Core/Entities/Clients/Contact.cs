using Manager.Core.Common;

namespace Manager.Core.Entities.Clients;

public class Contact : BaseEntity
{
    public required Guid ClientId { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Role { get; set; }
    public bool IsPrimary { get; set; } = false;
    
    public Client? Client { get; set; }
}
