using MongoDB.Driver;

namespace ProductService.Infrastructure.Data;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string collectionName);
}

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient mongoClient, string databaseName)
    {
        _database = mongoClient.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}
