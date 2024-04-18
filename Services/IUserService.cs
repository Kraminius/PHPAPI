using PHPAPI.Model;

namespace PHPAPI.Services
{
    public interface IUserService
    {
        Task CreateUserAsync(Model.User user);
        Task<Model.User> GetUserByUsernameAsync(string Username);
    }
}
