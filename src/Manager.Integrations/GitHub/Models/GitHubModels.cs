namespace Manager.Integrations.GitHub.Models;

public record CreateRepositoryRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public bool IsPrivate { get; init; } = false;
    public bool AutoInit { get; init; } = true;
}

public record CreateRepositoryResponse
{
    public required string Name { get; init; }
    public required string FullName { get; init; }
    public required string HtmlUrl { get; init; }
    public required string CloneUrl { get; init; }
    public required string DefaultBranch { get; init; }
}

public record SeedTemplateRequest
{
    public required string RepositoryName { get; init; }
    public required string TemplatePath { get; init; }
    public required Dictionary<string, string> Variables { get; init; }
}

public record ConfigurePagesRequest
{
    public required string RepositoryName { get; init; }
    public string Branch { get; init; } = "main";
    public string Path { get; init; } = "/";
}

public record ConfigurePagesResponse
{
    public required string Status { get; init; }
    public required string Url { get; init; }
}
