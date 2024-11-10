using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;

namespace PIMS.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region RegisterUser
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserRegistrationDTO userRegistrationDTO)
        {            
            var user = new User
            {
                Username = userRegistrationDTO.Username,
                PasswordSalt = userRegistrationDTO.PasswordSalt,
                PasswordHash = userRegistrationDTO.PasswordHash,
                RoleID = userRegistrationDTO.RoleID
            };

            try
            {
                await _userService.RegisterUserAsync(user);
                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Registration failed: {ex.Message}");
            }
        }
        #endregion

        #region AuthenticateUser
        [HttpGet("login")]
        public async Task<IActionResult> AuthenticateUser(string username, string password)
        {
            var user = await _userService.AuthenticateUserAsync(username, password);
            if (user == null) return Unauthorized("Invalid credentials");
            // Generate JWT Token
            var token = await _userService.GenerateJwtToken(user);
            return Ok(new { message = "Login successful", role = user.RoleID, token });                      
        }
        #endregion
    }
}
