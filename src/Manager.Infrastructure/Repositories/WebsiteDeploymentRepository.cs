using Manager.Core.Entities;
using Manager.Core.Enums;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Manager.Infrastructure.Repositories;

public interface IWebsiteDeploymentRepository
{
    Task<WebsiteDeployment?> GetByIdAsync(string id);
    Task<IEnumerable<WebsiteDeployment>> GetByProjectIdAsync(string projectId);
    Task<IEnumerable<WebsiteDeployment>> GetAllAsync();
    Task<IEnumerable<WebsiteDeployment>> FindAsync(Expression<Func<WebsiteDeployment, bool>> filter);
    Task<WebsiteDeployment> CreateAsync(WebsiteDeployment entity);
    Task<bool> UpdateAsync(string id, WebsiteDeployment entity);
    Task<bool> DeleteAsync(string id);
    Task<bool> UpdateStatusAsync(string id, JobStatus status, string? errorMessage = null);
    Task<WebsiteDeployment?> GetLatestByProjectIdAsync(string projectId);
}

public sealed class WebsiteDeploymentRepository : MongoRepository<WebsiteDeployment>, IWebsiteDeploymentRepository
{
    private readonly IMongoCollection<WebsiteDeployment> _collection;

    public WebsiteDeploymentRepository(IMongoCollection<WebsiteDeployment> collection) : base(collection)
    {
        _collection = collection;
    }

    public async Task<IEnumerable<WebsiteDeployment>> GetByProjectIdAsync(string projectId)
    {
        return await _collection
            .Find(x => x.ProjectId == projectId)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<WebsiteDeployment?> GetLatestByProjectIdAsync(string projectId)
    {
        return await _collection
            .Find(x => x.ProjectId == projectId)
            .SortByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateStatusAsync(string id, JobStatus status, string? errorMessage = null)
    {
        var filter = Builders<WebsiteDeployment>.Filter.Eq(x => x.Id, id);
        var update = Builders<WebsiteDeployment>.Update
            .Set(x => x.Status, status)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        if (errorMessage != null)
        {
            update = update.Set(x => x.ErrorMessage, errorMessage);
        }

        var result = await _collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }
}
