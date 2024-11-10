using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IUserService
    {
        //Task<User> AuthenticateUserAsync(string username, string password);
        //Task RegisterUserAsync(User user);

        Task<User> AuthenticateUserAsync(string username, string password);
        Task RegisterUserAsync(User user);
        bool IsAdmin(User user);
        Task<string> GenerateJwtToken(User user);  // Method to generate JWT token        
    }
}
