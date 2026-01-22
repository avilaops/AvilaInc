using Manager.Core.Common;
using System.Text.Json;

namespace Manager.Core.Entities.Projects;

public class ProjectSpec : BaseEntity
{
    public required Guid ProjectId { get; set; }
    public required string Brand { get; set; }
    public string? Vertical { get; set; }
    public string? Goal { get; set; }
    
    // JSON columns for complex data
    public string? CtaJson { get; set; }
    public string? ThemeJson { get; set; }
    public string? ContentJson { get; set; }
    public string? IntegrationsJson { get; set; }
    public string? DeployJson { get; set; }
    
    public Project? Project { get; set; }
    
    // Helper properties to work with JSON data
    public Dictionary<string, object>? GetCta() 
        => string.IsNullOrEmpty(CtaJson) ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(CtaJson);
    
    public void SetCta(Dictionary<string, object> cta) 
        => CtaJson = JsonSerializer.Serialize(cta);
    
    public Dictionary<string, object>? GetTheme() 
        => string.IsNullOrEmpty(ThemeJson) ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(ThemeJson);
    
    public void SetTheme(Dictionary<string, object> theme) 
        => ThemeJson = JsonSerializer.Serialize(theme);
    
    public Dictionary<string, object>? GetContent() 
        => string.IsNullOrEmpty(ContentJson) ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(ContentJson);
    
    public void SetContent(Dictionary<string, object> content) 
        => ContentJson = JsonSerializer.Serialize(content);
    
    public Dictionary<string, object>? GetIntegrations() 
        => string.IsNullOrEmpty(IntegrationsJson) ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(IntegrationsJson);
    
    public void SetIntegrations(Dictionary<string, object> integrations) 
        => IntegrationsJson = JsonSerializer.Serialize(integrations);
    
    public Dictionary<string, object>? GetDeploy() 
        => string.IsNullOrEmpty(DeployJson) ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(DeployJson);
    
    public void SetDeploy(Dictionary<string, object> deploy) 
        => DeployJson = JsonSerializer.Serialize(deploy);
}
