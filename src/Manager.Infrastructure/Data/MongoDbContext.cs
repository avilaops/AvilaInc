using MongoDB.Driver;

namespace Manager.Infrastructure.Data;

public interface IMongoDbContext
{
    IMongoDatabase GetDatabase(string databaseName);
    IMongoCollection<T> GetCollection<T>(string databaseName, string collectionName);
}

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoClient _client;
    private readonly Dictionary<string, IMongoDatabase> _databases;

    public MongoDbContext(string connectionString)
    {
        _client = new MongoClient(connectionString);
        _databases = new Dictionary<string, IMongoDatabase>();
    }

    public IMongoDatabase GetDatabase(string databaseName)
    {
        if (!_databases.ContainsKey(databaseName))
        {
            _databases[databaseName] = _client.GetDatabase(databaseName);
        }
        
        return _databases[databaseName];
    }

    public IMongoCollection<T> GetCollection<T>(string databaseName, string collectionName)
    {
        var database = GetDatabase(databaseName);
        return database.GetCollection<T>(collectionName);
    }
}
