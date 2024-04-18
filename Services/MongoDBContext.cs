using MongoDB.Driver;
using PHPAPI.Model;

namespace PHPAPI.Services
{
    public class MongoDBContext : IMongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(MongoDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public Task<DeleteResult> DeleteOneAsync<T>(FilterDefinition<T> filter)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> FindAsync<T>(FilterDefinition<T> filter)
        {
            throw new NotImplementedException();
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }

        public Task InsertOneAsync<T>(T document)
        {
            throw new NotImplementedException();
        }

    }
}