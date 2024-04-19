using MongoDB.Bson;
using PHPAPI.Model;

namespace PHPAPI.Services
{
    public interface ISupermarketService
    {
        Task CreateUserAsync(Model.Supermarket supermarket);

        Task<Model.Supermarket> GetSupermarketById(ObjectId id);
    }
}
