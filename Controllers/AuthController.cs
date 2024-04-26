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

            var existingUser = await _userService.GetUserByUsernameAsync(registrationInput.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already taken");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registrationInput.PasswordHash);

            var newUser = new User
            {
                Username = registrationInput.Username,
                PasswordHash = hashedPassword,
                Email = registrationInput.Email,
                Name = registrationInput.Name,
                HomeAddress = registrationInput.HomeAddress,
                WorkAddress = registrationInput.WorkAddress
            };

            await _userService.CreateUserAsync(newUser);
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

            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token, Username = user.Username });
        }
    }
}
