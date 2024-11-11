using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Exceptions;

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
        /// <summary>
        ///User Registration.
        /// </summary>
        /// <remarks>
        /// This endpoint allows you to create a new user by providing the required details such as username, password hash, password salt, and role ID.
        /// The role ID specifies whether the user is an Administrator (1) or a regular User (2).
        /// 
        /// **Sample Request**:        
        /// {
        ///   "username": "TestUser",
        ///   "passwordHash": "Test@123",
        ///   "passwordSalt": "salt123",
        ///   "roleID": 1
        /// }        
        /// 
        /// **Sample Response**:
        /// - `200 OK`: User successfully registered.        
        ///   "User registered successfully"        
        /// - `400 Bad Request`: Registration failed due to invalid data or duplicate username.        
        ///   "Registration failed: Username already exists"        
        /// - `500 Internal Server Error`: An unexpected error occurred during the registration process.                  
        /// </remarks>

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

        /// <summary>
        /// login of User
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     Get: /api/v{version}/User/login
        /// 
        /// Expected Input:
        /// - "username": Must be a valid username, e.g., "TestUser".
        /// - "password": e.g., "Test@123".
        /// - "version": API version as an integer, e.g., 1.
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///     "Login successful"
        /// 
        ///     401 Unauthorized
        ///     User not found.
        /// 
        ///     500 Internal Server Error
        ///     false
        /// 
        /// </remarks>
        /// <param name="username">The username of the user. Example: "TestUser"</param>
        /// <param name="password">The user's password. Example: "Test@123"</param>
        /// <param name="version">The version of the API. Example: 1</param>
        /// <response code="200">If user details are successfully retrieved.</response>
        /// <response code="400">If a parameter is missing or invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>

        [HttpGet("login")]
        public async Task<IActionResult> AuthenticateUser(string username, string password)
        {
            try
            {
                var user = await _userService.AuthenticateUserAsync(username, password);
                if (user == null)
                    return Unauthorized("Invalid credentials");
                // Generate JWT Token
                var token = await _userService.GenerateJwtToken(user);                
                return Ok(new { message = "Login successful", token });
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        #endregion
    }
}
