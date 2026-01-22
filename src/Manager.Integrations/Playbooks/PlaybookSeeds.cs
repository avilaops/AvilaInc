using System.Text.Json;
using Manager.Core.Entities.Orchestration;

namespace Manager.Integrations.Playbooks;

public static class PlaybookSeeds
{
    public static Playbook LandingPagePlaybook()
    {
        var steps = new object[]
        {
            new { Name = "ValidateSpec", Handler = "ValidateSpecStep", Config = new Dictionary<string, object>() },
            new { Name = "CreateRepo", Handler = "CreateRepoStep", Config = new Dictionary<string, object>() },
            new { Name = "SeedTemplate", Handler = "SeedTemplateStep", Config = new { templatePath = "/templates/landing-engine" } },
            new { Name = "ConfigurePages", Handler = "ConfigurePagesStep", Config = new Dictionary<string, object>() },
            new { Name = "TriggerBuild", Handler = "TriggerBuildStep", Config = new { workflowFile = "deploy.yml" } },
            new { Name = "EmitDNSInstructions", Handler = "EmitDNSInstructionsStep", Config = new Dictionary<string, object>() },
            new { Name = "MarkLive", Handler = "MarkLiveStep", Config = new Dictionary<string, object>() }
        };

        return new Playbook
        {
            Name = "Landing Page (GitHub Pages)",
            Code = "LANDING_GITHUB_PAGES",
            Description = "Creates a landing page project using GitHub Pages deployment",
            StepsJson = JsonSerializer.Serialize(steps),
            IsActive = true,
            Version = 1
        };
    }

    public static Playbook WebsitePlaybook()
    {
        var steps = new object[]
        {
            new { Name = "ValidateSpec", Handler = "ValidateSpecStep", Config = new Dictionary<string, object>() },
            new { Name = "CreateRepo", Handler = "CreateRepoStep", Config = new Dictionary<string, object>() },
            new { Name = "SeedTemplate", Handler = "SeedTemplateStep", Config = new { templatePath = "/templates/website-engine" } },
            new { Name = "ConfigurePages", Handler = "ConfigurePagesStep", Config = new Dictionary<string, object>() },
            new { Name = "TriggerBuild", Handler = "TriggerBuildStep", Config = new { workflowFile = "deploy.yml" } }
        };

        return new Playbook
        {
            Name = "Multi-page Website",
            Code = "WEBSITE_GITHUB_PAGES",
            Description = "Creates a multi-page website using GitHub Pages",
            StepsJson = JsonSerializer.Serialize(steps),
            IsActive = true,
            Version = 1
        };
    }

    public static List<Playbook> GetAllPlaybooks()
    {
        return new List<Playbook>
        {
            LandingPagePlaybook(),
            WebsitePlaybook()
        };
    }
}
