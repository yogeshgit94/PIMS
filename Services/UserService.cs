using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task RegisterUserAsync(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                throw new InvalidOperationException("Username already exists");

            user.PasswordHash = HashPassword(user.PasswordHash); // Simple password hashing
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        //authentication user
        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            var hashedPassword = HashPassword(password);
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hashedPassword);
        }

        private string HashPassword(string password)
        {
            // Implement password hashing logic (for example, using SHA-256)
            return password; // Simplified for example purposes
        }       
    }
}
