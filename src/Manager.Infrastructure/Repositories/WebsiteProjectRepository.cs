using Manager.Core.Entities;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Manager.Infrastructure.Repositories;

public interface IWebsiteProjectRepository
{
    Task<WebsiteProject?> GetByIdAsync(string id);
    Task<WebsiteProject?> GetByRequestIdAsync(string requestId);
    Task<WebsiteProject?> GetBySubdomainAsync(string subdomain);
    Task<IEnumerable<WebsiteProject>> GetAllAsync();
    Task<IEnumerable<WebsiteProject>> FindAsync(Expression<Func<WebsiteProject, bool>> filter);
    Task<WebsiteProject> CreateAsync(WebsiteProject entity);
    Task<bool> UpdateAsync(string id, WebsiteProject entity);
    Task<bool> DeleteAsync(string id);
    Task<bool> UpdateContentAsync(string id, WebsiteContent content);
    Task<bool> PublishAsync(string id, string liveUrl);
}

public sealed class WebsiteProjectRepository : MongoRepository<WebsiteProject>, IWebsiteProjectRepository
{
    private readonly IMongoCollection<WebsiteProject> _collection;

    public WebsiteProjectRepository(IMongoCollection<WebsiteProject> collection) : base(collection)
    {
        _collection = collection;
    }

    public async Task<WebsiteProject?> GetByRequestIdAsync(string requestId)
    {
        return await _collection
            .Find(x => x.RequestId == requestId)
            .FirstOrDefaultAsync();
    }

    public async Task<WebsiteProject?> GetBySubdomainAsync(string subdomain)
    {
        return await _collection
            .Find(x => x.Subdomain == subdomain)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateContentAsync(string id, WebsiteContent content)
    {
        var filter = Builders<WebsiteProject>.Filter.Eq(x => x.Id, id);
        var update = Builders<WebsiteProject>.Update
            .Set(x => x.Content, content)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> PublishAsync(string id, string liveUrl)
    {
        var filter = Builders<WebsiteProject>.Filter.Eq(x => x.Id, id);
        var update = Builders<WebsiteProject>.Update
            .Set(x => x.IsPublished, true)
            .Set(x => x.LiveUrl, liveUrl)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }
}
