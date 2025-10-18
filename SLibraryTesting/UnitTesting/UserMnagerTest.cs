using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using SLibrary.Business.Managers;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;
using Shared;
using SLibrary.DataAccess.SUnitOfWork;
using System.Data;

namespace SLibraryTesting.UnitTesting
{
    public class UserManagerTest
    {
        private readonly Mock<IUnitOfWork> mockUow;
        private readonly Mock<ILogger<UserManager>> mockLogger;
        private readonly Mock<IUserRepository> mockUserRepo;
        private readonly UserManager manager;

        public UserManagerTest()
        {
            mockUow = new Mock<IUnitOfWork>();
            mockLogger = new Mock<ILogger<UserManager>>();
            mockUserRepo = new Mock<IUserRepository>();

            mockUow.SetupGet(u => u.DBUsers).Returns(mockUserRepo.Object);

            manager = new UserManager(mockUow.Object, mockLogger.Object);
        }

        [Fact]
        public void Add_NewUser()
        {
            var newUser = new dtoNewUser { Username = "TestUser", Email = "test@example.com", Password = "pass" };

            manager.Add(newUser);

            mockUserRepo.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
            mockUow.Verify(u => u.Complete(), Times.Once);
        }

        [Fact]
        public void Validatelogin_SouldReturnTrue_WhenPasswordCorrect()
        {
            var login = new dtoLogin { Username = "TestUser", password = "pass" };
            var user = new User { Id = Guid.NewGuid(), Password = "hashed" };

            mockUserRepo.Setup(r => r.GetByUsername("TestUser")).Returns(user);
            mockUserRepo.Setup(r => r.VerifyPassword("pass", user.Id.ToString(), "hashed")).Returns(true);

            var result = manager.Validatelogin(login);

            Assert.True(result);
        }

        [Fact]
        public void Validatelogin_ShouldReturnFalse_whenPasswordIncorrect()
        {
            var login = new dtoLogin { Username = "TestUser", password = "wrong" };
            var user = new User { Id = Guid.NewGuid(), Password = "hashed" };

            mockUserRepo.Setup(r => r.GetByUsername("TestUser")).Returns(user);
            mockUserRepo.Setup(r => r.VerifyPassword("wrong", user.Id.ToString(), "hashed")).Returns(false);

            var result = manager.Validatelogin(login);

            Assert.False(result);
        }

        [Fact]
        public void Validatelogin_ShouldRetrunFalse_WhenUserNotFound()
        {
            var login = new dtoLogin { Username = "MissingUser", password = "pass" };
            mockUserRepo.Setup(r => r.GetByUsername("MissingUser")).Returns((User)null);

            var result = manager.Validatelogin(login);

            Assert.False(result);
        }

        [Fact]
        public void GetByUsername_ShouldReturnUser_WhenUserExists()
        {
            var user = new User { Id = Guid.NewGuid(), Username = "TestUser", Role = Role.User };
            mockUserRepo.Setup(r => r.GetByUsername("TestUser")).Returns(user);

            var result = manager.GetByUsername("TestUser");

            Assert.NotNull(result);
            Assert.Equal("TestUser", result.Username);
            Assert.Equal(Role.User, result.Role);
        }

        [Fact]
        public void GetByUsername_ShouldReturnNull_WhenUserNotFound()
        {
            mockUserRepo.Setup(r => r.GetByUsername("MissingUser")).Returns((User)null);

            var result = manager.GetByUsername("MissingUser");

            Assert.Null(result);
        }

        [Fact]
        public void Delete_WhenRepoDeletes_ReturnSuccessMsg()
        {
            mockUserRepo.Setup(r => r.Delete("TestUser")).Returns(true);

            var result = manager.Delete("TestUser");

            Assert.Equal("User deleted successfully", result);
            mockUow.Verify(u => u.Complete(), Times.Once);
        }

        [Fact]
        public void Delete_WhenRepoFails_ReturnFailureMsg()
        {
            mockUserRepo.Setup(r => r.Delete("TestUser")).Returns(false);

            var result = manager.Delete("TestUser");

            Assert.Equal("Failed to delete user", result);
            mockUow.Verify(u => u.Complete(), Times.Never);
        }

        [Fact]
        public void GetAllUsers_ShouldReturnListOfDtos()
        {
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Username = "User1", Role = Role.User },
                new User { Id = Guid.NewGuid(), Username = "User2", Role = Role.Admin }
            };
            mockUserRepo.Setup(r => r.GetUsers()).Returns(users);

            var result = manager.GetAllUsers();

            Assert.Equal(2, result.Count);
            Assert.Equal("User1", result[0].Username);
            Assert.Equal(Role.Admin, result[1].Role);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_WhenVerified()
        {
            var id = Guid.NewGuid().ToString();
            mockUserRepo.Setup(r => r.VerifyPassword("pass", id, "hash")).Returns(true);

            var result = manager.VerifyPassword("pass", id, "hash");

            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_WhenNotVerified()
        {
            var id = Guid.NewGuid().ToString();
            mockUserRepo.Setup(r => r.VerifyPassword("pass", id, "hash")).Returns(false);

            var result = manager.VerifyPassword("pass", id, "hash");

            Assert.False(result);
        }
    }
}
