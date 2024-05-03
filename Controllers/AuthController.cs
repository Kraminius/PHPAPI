using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

            if(registrationInput.Username.Length < 4 || registrationInput.Username.Length > 20)
            {
                return BadRequest("Username must be between 4 and 20 characters");
            }

            if(registrationInput.PasswordHash.Length < 8 || registrationInput.PasswordHash.Length > 20)
            {
                return BadRequest("Password must be between 8 and 20 characters");
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
        public async Task<IActionResult> Login([FromBody] LoginModel loginInput)
        {
            var user = await _userService.GetUserByUsernameAsync(loginInput.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            if (!VerifyPassword(loginInput.Password, user.PasswordHash, user.Salt))
            {
                return Unauthorized("Invalid username or password");
            }

            var token = UserService.GenerateJwtToken(user.Username);
            return Ok(new { Token = token, Username = user.Username });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Invalid token data");
            }

            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new { Username = user.Username, Email = user.Email, Name = user.Name, HomeAddress = user.HomeAddress, WorkAddress = user.WorkAddress});
        }

    private bool VerifyPassword(string password, string storedHash, byte[] salt)
        {
            var hashOfInput = Convert.ToBase64String(HashPassword(password, salt));
            return hashOfInput == storedHash;
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
            int iterations = 600000;
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32); // Creates a 256-bit hash
        }

        private void SetPassword(User user, string password)
        {
            byte[] salt = GenerateSalt();
            user.Salt = salt;
            user.PasswordHash = Convert.ToBase64String(HashPassword(password, salt));
        }
    }
}
