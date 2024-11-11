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
using Services.Exceptions;


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
            //var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            //if (user == null) return null;

            //var hashedPassword = HashPassword(password, user.PasswordSalt);
            //return user.PasswordHash == hashedPassword ? user : null;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {                
                throw new AuthenticationException("User not found.");
            }            
            var hashedPassword = HashPassword(password, user.PasswordSalt);
            if (user.PasswordHash != hashedPassword)
            {             
                throw new AuthenticationException("Invalid password.");
            }
            return user;

        }
        #endregion

        #region GenerateSalt
        // Generate a salt for password hashing
        public string GenerateSalt()
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
        public string HashPassword(string password, string salt)
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
            var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.RoleID == 1 ? "Administrator" : "User")
        };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);                       
        }
        #endregion        
    }
}
