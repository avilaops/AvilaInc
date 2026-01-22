using Microsoft.AspNetCore.Components;

namespace Landing.Services;

/// <summary>
/// Serviço para coletar e gerenciar contexto de rastreamento (UTM, referrer, etc.)
/// </summary>
public class TrackingContext
{
    private readonly NavigationManager _navigationManager;

    public TrackingContext(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    /// <summary>
    /// Extrai parâmetros UTM da URL atual
    /// </summary>
    public UtmParameters GetUtmParameters()
    {
        var uri = new Uri(_navigationManager.Uri);
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        return new UtmParameters
        {
            Source = GetQueryValue(query, "utm_source"),
            Medium = GetQueryValue(query, "utm_medium"),
            Campaign = GetQueryValue(query, "utm_campaign"),
            Term = GetQueryValue(query, "utm_term"),
            Content = GetQueryValue(query, "utm_content")
        };
    }

    /// <summary>
    /// Obtém o caminho da página atual
    /// </summary>
    public string GetCurrentPagePath()
    {
        var uri = new Uri(_navigationManager.Uri);
        return uri.AbsolutePath;
    }

    /// <summary>
    /// Tenta obter o referrer (limitado pelo que o navegador permite)
    /// </summary>
    public string? GetReferrer()
    {
        // Em Blazor Server, o referrer pode ser limitado
        // Retornamos null por enquanto, pode ser implementado via JS se necessário
        return null;
    }

    private string? GetQueryValue(System.Collections.Generic.Dictionary<string, Microsoft.Extensions.Primitives.StringValues> query, string key)
    {
        return query.TryGetValue(key, out var values) ? values.FirstOrDefault() : null;
    }
}

/// <summary>
/// Parâmetros UTM extraídos da URL
/// </summary>
public class UtmParameters
{
    public string? Source { get; set; }
    public string? Medium { get; set; }
    public string? Campaign { get; set; }
    public string? Term { get; set; }
    public string? Content { get; set; }
}