using Octokit;
using Manager.Integrations.GitHub.Models;

namespace Manager.Integrations.GitHub;

public interface IGitHubService
{
    Task<CreateRepositoryResponse> CreateRepositoryAsync(CreateRepositoryRequest request);
    Task SeedTemplateAsync(SeedTemplateRequest request);
    Task<ConfigurePagesResponse> ConfigurePagesAsync(ConfigurePagesRequest request);
    Task<string> TriggerWorkflowAsync(string repositoryName, string workflowFileName);
}

public class GitHubService : IGitHubService
{
    private readonly GitHubClient _client;
    private readonly string _owner;

    public GitHubService(string token, string owner)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException("GitHub token is required", nameof(token));
        
        if (string.IsNullOrEmpty(owner))
            throw new ArgumentException("GitHub owner is required", nameof(owner));

        _client = new GitHubClient(new ProductHeaderValue("AvxManager"))
        {
            Credentials = new Credentials(token)
        };
        _owner = owner;
    }

    public async Task<CreateRepositoryResponse> CreateRepositoryAsync(CreateRepositoryRequest request)
    {
        var newRepo = new NewRepository(request.Name)
        {
            Description = request.Description,
            Private = request.IsPrivate,
            AutoInit = request.AutoInit
        };

        var repository = await _client.Repository.Create(newRepo);

        return new CreateRepositoryResponse
        {
            Name = repository.Name,
            FullName = repository.FullName,
            HtmlUrl = repository.HtmlUrl,
            CloneUrl = repository.CloneUrl,
            DefaultBranch = repository.DefaultBranch ?? "main"
        };
    }

    public async Task SeedTemplateAsync(SeedTemplateRequest request)
    {
        // Read template files from disk
        var templateFiles = Directory.GetFiles(request.TemplatePath, "*", SearchOption.AllDirectories);

        foreach (var filePath in templateFiles)
        {
            var relativePath = Path.GetRelativePath(request.TemplatePath, filePath);
            var fileContent = await File.ReadAllTextAsync(filePath);

            // Replace variables in content
            foreach (var (key, value) in request.Variables)
            {
                fileContent = fileContent.Replace($"{{{{{key}}}}}", value);
            }

            // Create file in repository
            await _client.Repository.Content.CreateFile(
                _owner,
                request.RepositoryName,
                relativePath,
                new CreateFileRequest($"Add {relativePath}", fileContent)
            );
        }
    }

    public async Task<ConfigurePagesResponse> ConfigurePagesAsync(ConfigurePagesRequest request)
    {
        try
        {
            // Get repository to construct Pages URL
            var repo = await _client.Repository.Get(_owner, request.RepositoryName);
            
            // Note: GitHub Pages API has changed. For now, return the expected URL
            // Pages must be manually enabled in repository settings
            var pagesUrl = $"https://{_owner}.github.io/{request.RepositoryName}/";

            return new ConfigurePagesResponse
            {
                Status = "manual_setup_required",
                Url = pagesUrl
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to configure GitHub Pages: {ex.Message}", ex);
        }
    }

    public async Task<string> TriggerWorkflowAsync(string repositoryName, string workflowFileName)
    {
        try
        {
            var repo = await _client.Repository.Get(_owner, repositoryName);
            var defaultBranch = repo.DefaultBranch ?? "main";

            // Note: Workflow dispatch API requires specific permissions
            // For now, return a message indicating manual trigger is needed
            return $"Workflow {workflowFileName} should be triggered manually on {defaultBranch}";
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to trigger workflow: {ex.Message}", ex);
        }
    }
}
