using Manager.Core.Entities;
using Manager.Core.Enums;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Manager.Infrastructure.Repositories;

public interface IWebsiteRequestRepository
{
    Task<WebsiteRequest?> GetByIdAsync(string id);
    Task<IEnumerable<WebsiteRequest>> GetAllAsync();
    Task<IEnumerable<WebsiteRequest>> FindAsync(Expression<Func<WebsiteRequest, bool>> filter);
    Task<WebsiteRequest> CreateAsync(WebsiteRequest entity);
    Task<bool> UpdateAsync(string id, WebsiteRequest entity);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<WebsiteRequest>> GetByStatusAsync(WebsiteRequestStatus status);
    Task<bool> UpdateStatusAsync(string id, WebsiteRequestStatus status, string? errorMessage = null);
    Task<long> CountByStatusAsync(WebsiteRequestStatus status);
}

public sealed class WebsiteRequestRepository : MongoRepository<WebsiteRequest>, IWebsiteRequestRepository
{
    private readonly IMongoCollection<WebsiteRequest> _collection;

    public WebsiteRequestRepository(IMongoCollection<WebsiteRequest> collection) : base(collection)
    {
        _collection = collection;
    }

    public async Task<IEnumerable<WebsiteRequest>> GetByStatusAsync(WebsiteRequestStatus status)
    {
        return await _collection
            .Find(x => x.Status == status)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(string id, WebsiteRequestStatus status, string? errorMessage = null)
    {
        var filter = Builders<WebsiteRequest>.Filter.Eq(x => x.Id, id);
        var update = Builders<WebsiteRequest>.Update
            .Set(x => x.Status, status)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        if (errorMessage != null)
        {
            update = update.Set(x => x.ErrorMessage, errorMessage);
        }

        var result = await _collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<long> CountByStatusAsync(WebsiteRequestStatus status)
    {
        return await _collection.CountDocumentsAsync(x => x.Status == status);
    }
}
