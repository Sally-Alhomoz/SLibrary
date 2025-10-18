using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SLibrary.DataAccess;
using SLibrary.DataAccess.Models;
using SLibrary.DataAccess.Repositories;
using Shared;
using Xunit;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using Microsoft.Extensions.Configuration.UserSecrets;


namespace SLibraryTesting.IntegerationTesting
{
    public class UserRepoTest
    {
        private readonly SLibararyDBContext _db;
        private readonly DBUserRepository _repo;
        private readonly ILogger<DBUserRepository> _logger;

        public UserRepoTest()
        {
            var options = new DbContextOptionsBuilder<SLibararyDBContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _db = new SLibararyDBContext(options);

            var id = Guid.NewGuid();

            var testUser = new User
            {
                Id = id,
                Username = "admin",
                Email = "admin@example.com",
                Password = HashPasswordForTest("123", id),
                Role = Role.Admin
            };
            _db.Users.Add(testUser);
            _db.SaveChanges();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<DBUserRepository>();

            _repo = new DBUserRepository(_db, _logger);
        }

        [Fact]
        public void Add_ShouldAddNewUser()
        {
            Guid id = Guid.NewGuid();
            var user = new User
            {
                Id = id,
                Username = "user1",
                Email = "user1@example.com",
                Password = HashPasswordForTest("password", id),
                Role = Role.User
            };

            _repo.Add(user);
            _db.SaveChanges();

            var result = _db.Users.FirstOrDefault(u => u.Username == "user1");
            Assert.NotNull(result);
            Assert.NotEqual("password", result.Password);
        }

        [Fact]
        public void Add_ShouldthrowException_WhenUserExists()
        {
            var existing = _db.Users.First();
            var duplicate = new User
            {
                Id = existing.Id,
                Username = "admin",
                Email = "admin@example.com",
                Password = HashPasswordForTest("123", existing.Id),
                Role = Role.Admin
            };

            Assert.Throws<Exception>(() => _repo.Add(duplicate));
        }

        [Fact]
        public void GetByUsername_ShouldReturnUserWhenExists()
        {
            var result = _repo.GetByUsername("admin");
            Assert.NotNull(result);
            Assert.Equal("admin", result.Username);
        }

        [Fact]
        public void GetByUsername_ShouldreturnNull_WhenNotExists()
        {
            var result = _repo.GetByUsername("nonexistent");
            Assert.Null(result);
        }

        [Fact]
        public void GetUsers_ShouldReturnAllUsers()
        {
            var users = _repo.GetUsers();
            Assert.NotEmpty(users);
        }

        [Fact]
        public void Delete_ShouldReturnTrue_WhenExists()
        {
            var result = _repo.Delete("admin");
            _db.SaveChanges();

            Assert.True(result);
            Assert.Null(_db.Users.FirstOrDefault(u => u.Username == "admin"));
        }

        [Fact]
        public void Delete_ShouldReturnFalse_WhenUserNotFound()
        {
            var result = _repo.Delete("unknown");
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_ShouldRetrunTrue_CorrectPassword()
        {
            var user = _db.Users.First();
            bool result = _repo.VerifyPassword("123", user.Id.ToString(), _repo.GetByUsername("admin").Password);
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_ShouldRetrunFalse_WrongPassword()
        {
            var user = _db.Users.First();
            bool result = _repo.VerifyPassword("wrongpass", user.Id.ToString(), user.Password);
            Assert.False(result);
        }

        private string HashPasswordForTest(string pass, Guid id)
        {
            byte[] userid = Encoding.UTF8.GetBytes(id.ToString());
            byte[] hashed = KeyDerivation.Pbkdf2(
                password: pass,
                salt: userid,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32);
            return Convert.ToBase64String(hashed);
        }
    }
}
