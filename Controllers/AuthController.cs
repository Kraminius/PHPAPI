using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PHPAPI.Model;
using PHPAPI.Services;

namespace PHPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User registrationInput)
        {
            

            if (string.IsNullOrEmpty(registrationInput.Username) || string.IsNullOrEmpty(registrationInput.PasswordHash) ||
                string.IsNullOrEmpty(registrationInput.Email) || string.IsNullOrEmpty(registrationInput.Name) || 
                string.IsNullOrEmpty(registrationInput.HomeAddress) || string.IsNullOrEmpty(registrationInput.WorkAddress))
            {
                return BadRequest("Missing required fields");
            }

            byte[] salt = GenerateSalt();
            string hashedPassword = Convert.ToBase64String(HashPassword(registrationInput.PasswordHash, salt));

            var existingUser = await _userService.GetUserByUsernameAsync(registrationInput.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already taken");
            }

            var newUser = new User
            {
                Username = registrationInput.Username,
                PasswordHash = hashedPassword,
                Salt = salt,
                Email = registrationInput.Email,
                Name = registrationInput.Name,
                HomeAddress = registrationInput.HomeAddress,
                WorkAddress = registrationInput.WorkAddress
            };

            

            await _userService.CreateUserAsync(newUser);
            Console.WriteLine(newUser.Name + " was created successfully.");
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginInput)
        {
            var user = await _userService.GetUserByUsernameAsync(loginInput.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginInput.PasswordHash, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password");
            }

            var token = UserService.GenerateJwtToken(user.Username);
            return Ok(new { Token = token, Username = user.Username });
        }

        private byte[] GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);
            return salt;
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            int iterations = 5000; //TODO: TEST PERFORMANCE, LOWER = QUICKER, HIGHER = SLOWER BUT MORE SECURE
            var hashAlgorithm = HashAlgorithmName.SHA256;

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, hashAlgorithm);
            return pbkdf2.GetBytes(20);
        }
    }
}
