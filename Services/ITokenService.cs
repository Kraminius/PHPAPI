using PHPAPI.Model;

namespace PHPAPI.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}