using System.Text.Json;
using Microsoft.Extensions.Logging;
using Manager.Integrations.GitHub;
using Manager.Integrations.GitHub.Models;

namespace Manager.Integrations.Playbooks;

public interface IPlaybookStep
{
    Task<Dictionary<string, object>> ExecuteAsync(Dictionary<string, object> context);
}

public class CreateRepoStep : IPlaybookStep
{
    private readonly IGitHubService _gitHubService;
    private readonly ILogger<CreateRepoStep> _logger;

    public CreateRepoStep(IGitHubService gitHubService, ILogger<CreateRepoStep> logger)
    {
        _gitHubService = gitHubService;
        _logger = logger;
    }

    public async Task<Dictionary<string, object>> ExecuteAsync(Dictionary<string, object> context)
    {
        var projectName = context["projectName"].ToString();
        var description = context.GetValueOrDefault("description")?.ToString();

        _logger.LogInformation("Creating repository for project {ProjectName}", projectName);

        var request = new CreateRepositoryRequest
        {
            Name = projectName!,
            Description = description,
            IsPrivate = false,
            AutoInit = true
        };

        var response = await _gitHubService.CreateRepositoryAsync(request);

        context["repositoryName"] = response.Name;
        context["repositoryUrl"] = response.HtmlUrl;
        context["cloneUrl"] = response.CloneUrl;
        context["defaultBranch"] = response.DefaultBranch;

        _logger.LogInformation("Repository created: {RepositoryUrl}", response.HtmlUrl);

        return context;
    }
}

public class SeedTemplateStep : IPlaybookStep
{
    private readonly IGitHubService _gitHubService;
    private readonly ILogger<SeedTemplateStep> _logger;

    public SeedTemplateStep(IGitHubService gitHubService, ILogger<SeedTemplateStep> logger)
    {
        _gitHubService = gitHubService;
        _logger = logger;
    }

    public async Task<Dictionary<string, object>> ExecuteAsync(Dictionary<string, object> context)
    {
        var repositoryName = context["repositoryName"].ToString();
        var templatePath = context["templatePath"].ToString();
        var variablesJson = context.GetValueOrDefault("variables")?.ToString() ?? "{}";
        var variables = JsonSerializer.Deserialize<Dictionary<string, string>>(variablesJson) ?? new();

        _logger.LogInformation("Seeding template for repository {RepositoryName}", repositoryName);

        var request = new SeedTemplateRequest
        {
            RepositoryName = repositoryName!,
            TemplatePath = templatePath!,
            Variables = variables
        };

        await _gitHubService.SeedTemplateAsync(request);

        _logger.LogInformation("Template seeded successfully");

        return context;
    }
}

public class ConfigurePagesStep : IPlaybookStep
{
    private readonly IGitHubService _gitHubService;
    private readonly ILogger<ConfigurePagesStep> _logger;

    public ConfigurePagesStep(IGitHubService gitHubService, ILogger<ConfigurePagesStep> logger)
    {
        _gitHubService = gitHubService;
        _logger = logger;
    }

    public async Task<Dictionary<string, object>> ExecuteAsync(Dictionary<string, object> context)
    {
        var repositoryName = context["repositoryName"].ToString();
        var branch = context.GetValueOrDefault("branch")?.ToString() ?? "main";

        _logger.LogInformation("Configuring GitHub Pages for {RepositoryName}", repositoryName);

        var request = new ConfigurePagesRequest
        {
            RepositoryName = repositoryName!,
            Branch = branch
        };

        var response = await _gitHubService.ConfigurePagesAsync(request);

        context["pagesUrl"] = response.Url;
        context["pagesStatus"] = response.Status;

        _logger.LogInformation("GitHub Pages configured: {PagesUrl}", response.Url);

        return context;
    }
}

public class TriggerBuildStep : IPlaybookStep
{
    private readonly IGitHubService _gitHubService;
    private readonly ILogger<TriggerBuildStep> _logger;

    public TriggerBuildStep(IGitHubService gitHubService, ILogger<TriggerBuildStep> logger)
    {
        _gitHubService = gitHubService;
        _logger = logger;
    }

    public async Task<Dictionary<string, object>> ExecuteAsync(Dictionary<string, object> context)
    {
        var repositoryName = context["repositoryName"].ToString();
        var workflowFile = context.GetValueOrDefault("workflowFile")?.ToString() ?? "deploy.yml";

        _logger.LogInformation("Triggering build for {RepositoryName}", repositoryName);

        var result = await _gitHubService.TriggerWorkflowAsync(repositoryName!, workflowFile);

        context["buildTriggered"] = true;
        context["buildMessage"] = result;

        _logger.LogInformation("Build triggered: {Result}", result);

        return context;
    }
}
