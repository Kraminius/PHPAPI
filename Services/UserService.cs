using PHPAPI.Model;
using PHPAPI.Services;
using MongoDB.Driver;

public class UserService : IUserService
{
    private readonly MongoDbUserContext _dbContext;

    public UserService(MongoDbUserContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateUserAsync(User user)
    {
        await _dbContext.Users.InsertOneAsync(user);
    }

    public async Task<User> AuthenticateUser(string username, string password)
    {
        var filter = Builders<User>.Filter.Eq("Username", username);
        var user = await _dbContext.Users.Find(filter).FirstOrDefaultAsync();
        if (user != null && VerifyPasswordHash(password, user.PasswordHash))
        {
            return user;
        }
        return null;
    }

    private string CreatePasswordHash(string password)
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPasswordHash(string password, string storedHash)
    {
        return CreatePasswordHash(password) == storedHash;
    }

    public Task<User> GetUserByUsernameAsync(string username)
    {
        var filter = Builders<User>.Filter.Eq("Username", username);
        _ = _dbContext.FindUsersAsync(Builders<User>.Filter.Eq("Username", username));

        return _dbContext.Users.Find(filter).FirstOrDefaultAsync();
    }
}