using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
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
            // Sanitize and trim user input
            registrationInput.Username = SanitizeInput(registrationInput.Username?.Trim());
            registrationInput.PasswordHash = SanitizeInput(registrationInput.PasswordHash?.Trim());
            registrationInput.Email = SanitizeInput(registrationInput.Email?.Trim());
            registrationInput.Name = SanitizeInput(registrationInput.Name?.Trim());

            // Check for required fields being empty after sanitization and trimming
            if (string.IsNullOrEmpty(registrationInput.Username) ||
                string.IsNullOrEmpty(registrationInput.PasswordHash) ||
                string.IsNullOrEmpty(registrationInput.Email) ||
                string.IsNullOrEmpty(registrationInput.Name) ||
                registrationInput.Location == null || registrationInput.Location.Length == 0)
            {
                return BadRequest("Missing required fields");
            }

            // Length checks for username and password
            if (registrationInput.Username.Length < 4 || registrationInput.Username.Length > 20)
            {
                return BadRequest("Username must be between 4 and 20 characters");
            }

            if (registrationInput.PasswordHash.Length < 8 || registrationInput.PasswordHash.Length > 20)
            {
                return BadRequest("Password must be between 8 and 20 characters");
            }

            // Email validation
            if (!Regex.IsMatch(registrationInput.Email, @"^\S+@\S+\.\S+$"))
            {
                return BadRequest("Invalid email format");
            }

            // Sanitize and validate locations
            foreach (var location in registrationInput.Location)
            {
                // Sanitize location fields
                location.Address = SanitizeInput(location.Address?.Trim());
                location.LocationName = SanitizeInput(location.LocationName?.Trim());
                location.Logo = SanitizeInput(location.Logo?.Trim());

                // Check for invalid location data
                if (string.IsNullOrWhiteSpace(location.Address) ||
                    string.IsNullOrWhiteSpace(location.LocationName) ||
                    !double.TryParse(location.X.ToString(), out _) ||
                    !double.TryParse(location.Y.ToString(), out _))
                {
                    return BadRequest("Invalid location data");
                }
            }

            // Generate salt and hash password
            byte[] salt = GenerateSalt();
            string hashedPassword = Convert.ToBase64String(HashPassword(registrationInput.PasswordHash, salt));

            // Check if the username is already taken
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
                H3Index = "87f2b2266ffffff",
                Email = registrationInput.Email,
                Name = registrationInput.Name,
                Location = registrationInput.Location
            };

            // Create the new user in the database
            await _userService.CreateUserAsync(newUser);
            Console.WriteLine(newUser.Name + " was created successfully.");
            return Ok("User registered successfully");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginInput)
        {
            // Sanitize and trim the input
            loginInput.Username = SanitizeInput(loginInput.Username?.Trim());
            loginInput.Password = SanitizeInput(loginInput.Password?.Trim());

            // Attempt to find the user based on the sanitized username
            var user = await _userService.GetUserByUsernameAsync(loginInput.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            // Verify the password with sanitized input
            if (!VerifyPassword(loginInput.Password, user.PasswordHash, user.Salt))
            {
                return Unauthorized("Invalid username or password");
            }

            // Generate a token for the user
            var token = UserService.GenerateJwtToken(user.Username);
            return Ok(new
            {
                Token = token,
                Username = user.Username,
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Location = user.Location
            });
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

            return Ok(new {Id = user.Id, Username = user.Username, Email = user.Email, Name = user.Name, Location = user.Location});
        }

    private bool VerifyPassword(string password, string storedHash, byte[] salt)
        {
            var hashOfInput = Convert.ToBase64String(HashPassword(password, salt));
            return hashOfInput == storedHash;
        }

        private byte[] GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[32];
            rng.GetBytes(salt);
            return salt;
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            int iterations = 600000;
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32); // Creates a 256-bit hash
        }

        private string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Replace or remove dangerous characters
            return input.Replace(",", "")
                        .Replace(";", "")
                        .Replace("<", "")
                        .Replace(">", "")
                        .Replace("\"", "")
                        .Replace("'", "");
        }
    }
}
