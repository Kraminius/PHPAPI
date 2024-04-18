using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PHPAPI.Model;
using PHPAPI.Service;
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
            if (string.IsNullOrEmpty(registrationInput.Username))
            {
                return BadRequest("Username is required");
            }

            if (string.IsNullOrEmpty(registrationInput.Password))
            {
                return BadRequest("Password is required");
            }

            if (string.IsNullOrEmpty(registrationInput.Email))
            {
                return BadRequest("Email is required");
            }

            if (string.IsNullOrEmpty(registrationInput.Name))
            {
                return BadRequest("Name is required");
            }



            var salt = PasswordHasher.GenerateSalt();
            var hashedPassword = PasswordHasher.Hash(registrationInput.Password, salt);

            var newUser = new User(registrationInput.Username, registrationInput.Password, salt, registrationInput.Email, registrationInput.Name);


            await _userService.CreateUserAsync(newUser);
            return Ok("User registered");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User model)
        {
            var user = await _userService.GetUserByUsernameAsync(model.Username);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);

            if (!isPasswordValid)
            {
                return Unauthorized("Invalid username or password");
            }

            var token = _tokenService.GenerateToken(user);

            return Ok(new { token });
        }
    }
}