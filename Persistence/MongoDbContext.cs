using MongoDB.Driver;

namespace Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _db;

    public MongoDbContext(IMongoClient client, string dbName)
    {
        _db = client.GetDatabase(dbName);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _db.GetCollection<T>(collectionName);
    }
}
