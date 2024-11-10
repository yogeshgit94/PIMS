using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;


namespace Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        #region RegisterUserAsync
        public async Task RegisterUserAsync(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                throw new InvalidOperationException("Username already exists");

            var salt = GenerateSalt();
            user.PasswordSalt = salt;
            user.PasswordHash = HashPassword(user.PasswordHash, salt);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region AuthenticateUserAsync
        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            var hashedPassword = HashPassword(password, user.PasswordSalt);
            return user.PasswordHash == hashedPassword ? user : null;
        }
        #endregion

        #region GenerateSalt
        // Generate a salt for password hashing
        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }
        #endregion

        #region HashPassword
        // Hash password with salt
        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = $"{password}{salt}";
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashBytes);
            }
        }
        #endregion

        #region IsAdmin
        // Check if a user has admin privileges
        public bool IsAdmin(User user)
        {
            return user.RoleID == 1; // Assuming RoleID 1 is for Administrator
        }
        #endregion

        #region GenerateJwtToken
        public async Task<string> GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.RoleID == 1 ? "Administrator" : "User")
        };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtIssuer,
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion        
    }
}
