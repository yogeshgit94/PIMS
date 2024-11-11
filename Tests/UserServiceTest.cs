using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using ServiceContracts;
using Services;
using Services.Exceptions;
using Xunit;

namespace Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            // In-memory database setup for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);

            // IConfiguration ko mock karna
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config["Jwt:Key"]).Returns("Xh+J2c9YQj5D5S3eO+Q9BxFyW1FZ3PZ3y+P5yA==");
            _mockConfiguration.Setup(config => config["Jwt:Issuer"]).Returns("JWTAuthenticationServer");

            // UserService ko initialize karo mock dependencies ke sath
            _userService = new UserService(_context, _mockConfiguration.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldAddUser_WhenUserIsUnique()
        {
            // Arrange
            var newUser = new User
            {
                Username = "testyogesh",
                PasswordHash = "test@123",
                RoleID = 1
            };

            // Act
            await _userService.RegisterUserAsync(newUser);
            var addedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "testyogesh");

            // Assert
            Assert.NotNull(addedUser); 
            Assert.Equal("testyogesh", addedUser.Username); 
            Assert.NotNull(addedUser.PasswordHash); 
            Assert.NotNull(addedUser.PasswordSalt); 
        }


        [Fact]
        public async Task RegisterUserAsync_ShouldThrowException_WhenUsernameExists()
        {
            // Arrange
            var existingUser = new User
            {
                Username = "Yogeshwar",
                PasswordHash = "Yogesh@123",
                PasswordSalt = "1234", 
                RoleID = 1
            };
            await _context.Users.AddAsync(existingUser);
            await _context.SaveChangesAsync();

            var newUser = new User
            {
                Username = "Yogeshwar",
                PasswordHash = "Yogesh@123",
                PasswordSalt = "123", 
                RoleID = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterUserAsync(newUser));
        }





        //[Fact]
        //public async Task RegisterUserAsync_ShouldThrowException_WhenUsernameExists()
        //{            
        //    var existingUser = new User { Username = "Yogeshwar", PasswordHash = "Yogesh@123", RoleID = 1 };
        //    await _context.Users.AddAsync(existingUser);
        //    await _context.SaveChangesAsync();
        //    var newUser = new User { Username = "Yogeshwar", PasswordHash = "Yogesh@123", RoleID = 1 };            
        //    await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterUserAsync(newUser));
        //}

        [Fact]
        public async Task AuthenticateUserAsync_ShouldReturnUser_WhenCredentialsAreCorrect()
        {
            // Arrange
            var salt = _userService.GenerateSalt();
            var passwordHash = _userService.HashPassword("Yogesh@123", salt);

            var testUser = new User
            {
                Username = "Yogeshwar",
                PasswordHash = passwordHash,
                PasswordSalt = salt,
                RoleID = 1
            };
            _context.Users.Add(testUser);
            await _context.SaveChangesAsync();

            // Act
            var user = await _userService.AuthenticateUserAsync("Yogeshwar", "Yogesh@123");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("Yogeshwar", user.Username);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ShouldThrowAuthenticationException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var salt = _userService.GenerateSalt();
            var passwordHash = _userService.HashPassword("Yogesh@123", salt);

            var testUser = new User
            {
                Username = "Yogeshwar",
                PasswordHash = passwordHash,
                PasswordSalt = salt,
                RoleID = 1
            };
            _context.Users.Add(testUser);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<AuthenticationException>(() => _userService.AuthenticateUserAsync("Yogeshwar", "Yogesh#12"));
        }

        [Fact]
        public async Task AuthenticateUserAsync_ShouldThrowAuthenticationException_WhenUserNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<AuthenticationException>(() => _userService.AuthenticateUserAsync("nonexistentuser", "password"));
        }

        [Fact]
        public void IsAdmin_ShouldReturnTrue_WhenUserIsAdmin()
        {
            // Arrange
            var adminUser = new User { RoleID = 1 };

            // Act
            var isAdmin = _userService.IsAdmin(adminUser);

            // Assert
            Assert.True(isAdmin);
        }

        [Fact]
        public void IsAdmin_ShouldReturnFalse_WhenUserIsNotAdmin()
        {
            // Arrange
            var normalUser = new User { RoleID = 2 };

            // Act
            var isAdmin = _userService.IsAdmin(normalUser);

            // Assert
            Assert.False(isAdmin);
        }

        [Fact]
        public async Task GenerateJwtToken_ShouldReturnTokenString()
        {
            // Arrange
            var testUser = new User { Username = "Yogeshwar", RoleID = 1 };

            // Act
            var token = await _userService.GenerateJwtToken(testUser);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));
        }
    }
}