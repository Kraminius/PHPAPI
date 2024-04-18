using MongoDB.Driver;

namespace PHPAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<Model.User> _usersCollection;

        public UserService(IMongoDBContext context)
        {
            _usersCollection = context.GetCollection<Model.User>("Users");
        }

        public async Task CreateUserAsync(Model.User user)
        {
            await _usersCollection.InsertOneAsync(user);
        }

        public async Task<Model.User> GetUserByUsernameAsync(string Username)
        {
            return await _usersCollection.Find(u => u.Username == Username).FirstOrDefaultAsync();
        }
    }
}
