using Manager.Core.Entities;
using Manager.Infrastructure.Repositories;
using MongoDB.Driver;

namespace Manager.Infrastructure.Repositories;

// Nova interface MongoDB-based (não herda de IRepository<Company>)
public interface ICompanyMongoRepository
{
    Task<Company?> GetByIdAsync(string id);
    Task<Company?> GetByGooglePlaceIdAsync(string googlePlaceId);
    Task<Company?> GetByCnpjAsync(string cnpj);
    Task<List<Company>> GetAllAsync(string? source = null, string? query = null, int skip = 0, int limit = 50);
    Task<Company> CreateAsync(Company company);
    Task<Company> UpdateAsync(Company company);
    Task<bool> DeleteAsync(string id);
    Task<Company> UpsertByGooglePlaceIdAsync(Company company);
}

public class CompanyRepository : ICompanyMongoRepository
{
    private readonly IMongoCollection<Company> _collection;

    public CompanyRepository(IMongoCollection<Company> collection)
    {
        _collection = collection;
    }

    public async Task<Company?> GetByIdAsync(string id)
    {
        return await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Company?> GetByGooglePlaceIdAsync(string googlePlaceId)
    {
        return await _collection.Find(c => c.GooglePlaceId == googlePlaceId).FirstOrDefaultAsync();
    }

    public async Task<Company?> GetByCnpjAsync(string cnpj)
    {
        return await _collection.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync();
    }

    public async Task<List<Company>> GetAllAsync(string? source = null, string? query = null, int skip = 0, int limit = 50)
    {
        var filterBuilder = Builders<Company>.Filter;
        var filter = filterBuilder.Empty;

        if (!string.IsNullOrEmpty(source))
        {
            filter &= filterBuilder.Eq(c => c.Source, source);
        }

        if (!string.IsNullOrEmpty(query))
        {
            var queryFilter = filterBuilder.Or(
                filterBuilder.Regex(c => c.Name, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                filterBuilder.Regex(c => c.FantasyName, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                filterBuilder.Regex(c => c.City, new MongoDB.Bson.BsonRegularExpression(query, "i"))
            );
            filter &= queryFilter;
        }

        return await _collection.Find(filter)
            .SortByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<Company> CreateAsync(Company company)
    {
        company.CreatedAt = DateTime.UtcNow;
        company.UpdatedAt = DateTime.UtcNow;
        await _collection.InsertOneAsync(company);
        return company;
    }

    public async Task<Company> UpdateAsync(Company company)
    {
        company.UpdatedAt = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(c => c.Id == company.Id, company);
        return company;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<Company> UpsertByGooglePlaceIdAsync(Company company)
    {
        var existing = await GetByGooglePlaceIdAsync(company.GooglePlaceId!);
        
        if (existing != null)
        {
            company.Id = existing.Id;
            company.CreatedAt = existing.CreatedAt;
            return await UpdateAsync(company);
        }
        
        return await CreateAsync(company);
    }
}