using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace PIMS.Controllers
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

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(User user)
        {
            await _userService.RegisterUserAsync(user);
            return Ok("User registered successfully");
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateUser(string username, string password)
        {
            var user = await _userService.AuthenticateUserAsync(username, password);
            if (user == null) return Unauthorized("Invalid credentials");
            return Ok(user);
        }
    }
}
