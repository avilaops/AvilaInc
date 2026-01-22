using Manager.Core.Common;

namespace Manager.Core.Entities.Audit;

public class WebhookEvent : BaseEntity
{
    public required string Source { get; set; } // GitHub, Stripe, etc.
    public required string EventType { get; set; }
    public required string PayloadJson { get; set; }
    public string? Signature { get; set; }
    public bool IsProcessed { get; set; } = false;
    public DateTime? ProcessedAt { get; set; }
    public string? ProcessingResult { get; set; }
}
