using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

[BsonIgnoreExtraElements]
public class ClientFinance : MongoEntity
{
    [BsonElement("clientId")]
    public string ClientId { get; set; } = string.Empty;

    [BsonElement("clientName")]
    public string ClientName { get; set; } = string.Empty;

    [BsonElement("type")]
    public string Type { get; set; } = "payment"; // payment, invoice, refund

    [BsonElement("amount")]
    public decimal Amount { get; set; }

    [BsonElement("currency")]
    public string Currency { get; set; } = "BRL";

    [BsonElement("status")]
    public string Status { get; set; } = "pending";

    [BsonElement("paymentMethod")]
    public string? PaymentMethod { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("invoiceUrl")]
    public string? InvoiceUrl { get; set; }

    [BsonElement("dueDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DueDate { get; set; }

    [BsonElement("paidAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? PaidAt { get; set; }

    [BsonElement("stripePaymentIntentId")]
    public string? StripePaymentIntentId { get; set; }
}
