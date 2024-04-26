using MongoDB.Driver;
using PHPAPI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PHPAPI.Services
{
    public class MongoDbUserContext
    {
        private readonly IMongoDatabase _database;
        public IMongoCollection<User> Users { get; }

        public MongoDbUserContext(MongoDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
            Users = _database.GetCollection<User>("users");
        }

        public async Task InsertUserAsync(User user)
        {
            await Users.InsertOneAsync(user);
        }

        public async Task<IEnumerable<User>> FindUsersAsync(FilterDefinition<User> filter)
        {
            var results = await Users.FindAsync(filter);
            return results.ToEnumerable();
        }

        public async Task<DeleteResult> DeleteUserAsync(FilterDefinition<User> filter)
        {
            return await Users.DeleteOneAsync(filter);
        }
    }
}
