using Manager.Core.Common;

namespace Manager.Core.Entities.Clients;

public class BillingProfile : BaseEntity
{
    public required Guid ClientId { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentProvider { get; set; }
    public string? PaymentProviderId { get; set; }
    public string? BillingEmail { get; set; }
    public string? BillingAddress { get; set; }
    
    public Client? Client { get; set; }
}
