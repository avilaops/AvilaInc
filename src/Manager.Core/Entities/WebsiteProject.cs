using Manager.Core.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Manager.Core.Entities;

public sealed class WebsiteProject : MongoEntity
{
    [BsonElement("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [BsonElement("subdomain")]
    public string Subdomain { get; set; } = string.Empty;

    [BsonElement("templateType")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public WebsiteTemplateType TemplateType { get; set; }

    [BsonElement("themeTokens")]
    public ThemeTokens ThemeTokens { get; set; } = new();

    [BsonElement("content")]
    public WebsiteContent Content { get; set; } = new();

    [BsonElement("sections")]
    public List<string> Sections { get; set; } = new();

    [BsonElement("generatedHtml")]
    public string? GeneratedHtml { get; set; }

    [BsonElement("generatedCss")]
    public string? GeneratedCss { get; set; }

    [BsonElement("previewUrl")]
    public string? PreviewUrl { get; set; }

    [BsonElement("liveUrl")]
    public string? LiveUrl { get; set; }

    [BsonElement("isPublished")]
    public bool IsPublished { get; set; } = false;
}

public sealed class ThemeTokens
{
    [BsonElement("primaryColor")]
    public string PrimaryColor { get; set; } = "#1976d2";

    [BsonElement("secondaryColor")]
    public string SecondaryColor { get; set; } = "#7c4dff";

    [BsonElement("accentColor")]
    public string AccentColor { get; set; } = "#00c853";

    [BsonElement("fontFamily")]
    public string FontFamily { get; set; } = "Inter, sans-serif";

    [BsonElement("borderRadius")]
    public string BorderRadius { get; set; } = "16px";

    [BsonElement("spacing")]
    public string Spacing { get; set; } = "80px";
}

public sealed class WebsiteContent
{
    [BsonElement("businessName")]
    public string BusinessName { get; set; } = string.Empty;

    [BsonElement("heroHeadline")]
    public string HeroHeadline { get; set; } = string.Empty;

    [BsonElement("heroSubheadline")]
    public string HeroSubheadline { get; set; } = string.Empty;

    [BsonElement("services")]
    public List<ServiceItem> Services { get; set; } = new();

    [BsonElement("benefits")]
    public List<string> Benefits { get; set; } = new();

    [BsonElement("faqItems")]
    public List<FaqItem> FaqItems { get; set; } = new();

    [BsonElement("ctaText")]
    public string CtaText { get; set; } = "Fale Conosco no WhatsApp";

    [BsonElement("whatsapp")]
    public string WhatsApp { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("city")]
    public string City { get; set; } = string.Empty;

    [BsonElement("metaTitle")]
    public string MetaTitle { get; set; } = string.Empty;

    [BsonElement("metaDescription")]
    public string MetaDescription { get; set; } = string.Empty;
}

public sealed class ServiceItem
{
    [BsonElement("icon")]
    public string Icon { get; set; } = "🔧";

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
}

public sealed class FaqItem
{
    [BsonElement("question")]
    public string Question { get; set; } = string.Empty;

    [BsonElement("answer")]
    public string Answer { get; set; } = string.Empty;
}
