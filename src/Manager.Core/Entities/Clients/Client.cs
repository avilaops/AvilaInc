using Manager.Core.Common;

namespace Manager.Core.Entities.Clients;

public class Client : BaseEntity
{
    public required string Name { get; set; }
    public string? TaxId { get; set; }
    public string? Vertical { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public BillingProfile? BillingProfile { get; set; }
}
