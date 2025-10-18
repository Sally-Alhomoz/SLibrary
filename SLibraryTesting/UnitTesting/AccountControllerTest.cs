using Microsoft.Extensions.Logging;
using Moq;
using SLibrary.Business.Interfaces;
using SLibraryAPI.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Shared;
using SLibrary.DataAccess.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SLibraryTesting.UnitTesting
{
    public class AccountControllerTest
    {
        private readonly Mock<IUserManager> mockUserManager;
        private readonly Mock<IConfiguration> mockConfig;
        private readonly Mock<ILogger<AccountController>> mockLogger;
        private readonly AccountController controller;

        public AccountControllerTest()
        {

            mockUserManager = new Mock<IUserManager>();
            mockConfig = new Mock<IConfiguration>();
            mockLogger = new Mock<ILogger<AccountController>>();

            // Mock JWT Configuration values
            mockConfig.Setup(x => x["JWT:SecretKey"]).Returns("this_is_a_very_long_secret_key_1234567890123456");
            mockConfig.Setup(x => x["JWT:Issuer"]).Returns("TestIssuer");
            mockConfig.Setup(x => x["JWT:Audience"]).Returns("TestAudience");

            controller = new AccountController(
                mockUserManager.Object,
                mockConfig.Object,
                mockLogger.Object
            );
        }

        [Fact]
        public async Task Register_WhenUserAlreadyExists()
        {
            var newUser = new dtoNewUser { Username = "test", Password = "123" };
            mockUserManager.Setup(m => m.GetByUsername("test")).Returns(new Userdto { Username = "test" });

            var result = await controller.Create(newUser);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username already exists.", badRequest.Value);
        }

        [Fact]
        public async Task Register_WhenNewUser()
        {
            var newUser = new dtoNewUser { Username = "newuser", Password = "pass" };
            mockUserManager.Setup(m => m.GetByUsername("newuser")).Returns((Userdto)null);

            var result = await controller.Create(newUser);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully", okResult.Value);
            mockUserManager.Verify(m => m.Add(It.IsAny<dtoNewUser>()), Times.Once);
        }

        [Fact]
        public async Task Login_WhenInvalidCred()
        {
            var login = new dtoLogin { Username = "test", password = "wrong" };
            mockUserManager.Setup(m => m.Validatelogin(login)).Returns(false);

            var result = await controller.LogIn(login);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password", unauthorized.Value);
        }

        [Fact]
        public async Task Login_WhenValidCred()
        {
            var mockUserManager = new Mock<IUserManager>();
            var mockLogger = new Mock<ILogger<AccountController>>();

            var inMemorySettings = new Dictionary<string, string>
            {
               {"JWT:SecretKey", "this_is_a_very_long_secret_key_1234567890123456"},
               {"JWT:Issuer", "TestIssuer"},
               {"JWT:Audience", "TestAudience"}

            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var controller = new AccountController(mockUserManager.Object, config, mockLogger.Object);

            var login = new dtoLogin { Username = "admin", password = "123" };

            mockUserManager.Setup(m => m.Validatelogin(login)).Returns(true);
            mockUserManager.Setup(m => m.GetByUsername("admin")).Returns(new Userdto
            {
                Username = "admin",
                Role = Role.Admin
            });

            var result = await controller.LogIn(login);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            Assert.True(root.TryGetProperty("token", out var tokenProp));
            Assert.True(root.TryGetProperty("expiration", out var expProp));

            Assert.False(string.IsNullOrEmpty(tokenProp.GetString()));
            Assert.False(string.IsNullOrEmpty(expProp.GetDateTime().ToString()));
        }


        [Fact]
        public async Task Read_ReturnsListOfUsers()
        {
            mockUserManager.Setup(m => m.GetAllUsers()).Returns(new List<Userdto>
            {
                new Userdto { Username = "test" }
            });

            var result = await controller.Read();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var users = Assert.IsAssignableFrom<List<Userdto>>(okResult.Value);
            Assert.Single(users);
        }


        [Fact]
        public async Task Delete_WhenNonAdmin()
        {
            var userName = "target";
            var currentUser = "client";

            mockUserManager.Setup(m => m.GetByUsername(currentUser))
                .Returns(new Userdto { Username = currentUser, Role = Role.User });

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, currentUser)
                }))
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.Delete(userName);

            var forbidden = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, forbidden.StatusCode);
            Assert.Equal("Only admins can delete users.", forbidden.Value);
        }

        [Fact]
        public async Task Delete_WhenSuccess()
        {
            var usernameToDelete = "target";
            var adminUsername = "admin";

            mockUserManager.Setup(m => m.GetByUsername(adminUsername))
                .Returns(new Userdto { Username = adminUsername, Role = Role.Admin });

            mockUserManager.Setup(m => m.Delete(usernameToDelete))
                .Returns("User deleted successfully");

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, adminUsername)
                }))
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.Delete(usernameToDelete);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User deleted successfully", okResult.Value);
        }
    }
}
