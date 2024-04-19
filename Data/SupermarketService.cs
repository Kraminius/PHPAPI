using MongoDB.Driver;

namespace PHPAPI.Services
{
    public class SupermarketService : ISupermarketService
    {
        private readonly IMongoCollection<Model.Supermarket> _usersCollection;

        public SupermarketService(IMongoDBContext context)
        {
            _usersCollection = context.GetCollection<Model.Supermarket>("Supermarkets");
        }

        public async Task CreateUserAsync(Model.Supermarket supermarket)
        {
            await _usersCollection.InsertOneAsync(supermarket);
        }

        public async Task<Model.Supermarket> GetUserByUsernameAsync(string Id)
        {
            return await _usersCollection.Find(u => u.Id == Id).FirstOrDefaultAsync();
        }
    }
}
