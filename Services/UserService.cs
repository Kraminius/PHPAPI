using PHPAPI.Model;
using PHPAPI.Services;
using MongoDB.Driver;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Text;


public class UserService : IUserService
{
    private readonly MongoDBService _mongoDBService;

    public UserService(MongoDBService mongoDBService)
    {
        _mongoDBService = mongoDBService;
    }

    public async Task CreateUserAsync(User user)
    {
        try
        {
            await _mongoDBService.Users.InsertOneAsync(user);
        }
        catch (MongoException ex)
        {
            Console.Error.WriteLine($"Error creating user: {ex.Message}");
            Console.Error.WriteLine(ex.StackTrace);
        }
    }

    public async Task<User> AuthenticateUser(string username, string password)
    {
        var filter = Builders<User>.Filter.Eq("Username", username);
        var user = await _mongoDBService.Users.Find(filter).FirstOrDefaultAsync();
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
        _ = _mongoDBService.FindUsersAsync(Builders<User>.Filter.Eq("Username", username));

        return _mongoDBService.Users.Find(filter).FirstOrDefaultAsync();
        }

    public static string GenerateJwtToken(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var keyPath = "/container-secure-dir/private_key.pem";
        RSA rsa = null;  // Declare rsa outside of using block

        try
        {
            rsa = new RSACryptoServiceProvider();
            string privateKeyContent = File.ReadAllText(keyPath);
            rsa.ImportFromPem(privateKeyContent.ToCharArray());

            var credentials = new SigningCredentials(
                new RsaSecurityKey(rsa),
                SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username)
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = credentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An error occurred while generating JWT: {ex.Message}");
            return null;
        }
    }


    public static RsaSecurityKey ReadPublicKey(string pemFileContent)
    {
        using (var reader = new StringReader(pemFileContent))
        {
            var pemReader = new PemReader(reader);
            var publicKeyParameters = (RsaKeyParameters)pemReader.ReadObject();
            var rsa = RSA.Create();
            var rsaParameters = new RSAParameters
            {
                Modulus = publicKeyParameters.Modulus.ToByteArrayUnsigned(),
                Exponent = publicKeyParameters.Exponent.ToByteArrayUnsigned()
            };
            rsa.ImportParameters(rsaParameters);
            return new RsaSecurityKey(rsa);
        }
    }

  
}