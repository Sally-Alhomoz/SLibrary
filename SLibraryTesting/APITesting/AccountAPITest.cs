using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SLibraryAPI.Controllers;
using Shared;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Authentication;
using SLibrary.Business.Interfaces;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace SLibraryTesting.APITesting
{
    public class AccountAPITest : IClassFixture<WebApplicationFactory<AccountController>>
    {
        private readonly HttpClient _client;
        private readonly Mock<IUserManager> _mockUserManager;
        public AccountAPITest(WebApplicationFactory<AccountController> factory)
        {
            _mockUserManager = new Mock<IUserManager>();

            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthentication>("Test", options => { });

                    // Replace IBookManager with mock
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IUserManager));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddSingleton(_mockUserManager.Object);
                });
            }).CreateClient();
        }

        [Fact]
        public async Task CreateNewUser_ReturnOk_WhenSuccess()
        {
            var user = new dtoNewUser { Username = "TestUser", Password = "password", Email = "test@gmail.com" };
            _mockUserManager.Setup(m => m.Add(user));

            var response = await _client.PostAsJsonAsync("/api/account/Register", user);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var msg = await response.Content.ReadAsStringAsync();
            Assert.Contains("User registered successfully", msg);
        }

        [Fact]
        public async Task CreateNewUser_ReturnBadRequest_WhenFail()
        {
            var user = new Userdto { Username = "TestUser", Role=0,Id = Guid.NewGuid()};
            _mockUserManager.Setup(m => m.GetByUsername("TestUser")).Returns(user);

            var response = await _client.PostAsJsonAsync("/api/account/Register", user);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsToken_WhenCredentialsValid()
        {
            var login = new dtoLogin { Username = "admin", password = "Pass123" };
            var user = new Userdto { Id = Guid.NewGuid(), Username = "admin", Role = Role.Admin };

            _mockUserManager.Setup(m => m.Validatelogin(It.IsAny<dtoLogin>())).Returns(true);
            _mockUserManager.Setup(m => m.GetByUsername(It.IsAny<string>())).Returns(user);

            var response = await _client.PostAsJsonAsync("/api/account/Login", login);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            Assert.Contains("token", json);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsInvalid()
        {
            _mockUserManager.Setup(m => m.Validatelogin(It.IsAny<dtoLogin>())).Returns(false);

            var loginPayload = new { Username = "admin", password = "WrongPass" };
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", "admin"); // fake token

            var response = await _client.PostAsJsonAsync("/api/account/Login", loginPayload);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var msg = await response.Content.ReadAsStringAsync();
            Assert.Contains("Invalid username or password", msg);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk_WhenAuthorized()
        {
            var users = new List<Userdto>
            {
               new() { Username = "user1" },
               new() { Username = "user2" }
            };

            _mockUserManager.Setup(m => m.GetAllUsers()).Returns(users);

            // Fake token for testing
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "FakeJWTToken");

            var response = await _client.GetAsync("/api/account");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<List<Userdto>>();
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task DeleteUser_ReturnsForbidden_WhenNotAdmin()
        {
            var currentUser = new Userdto { Username = "user1", Role = Role.User, Id = Guid.NewGuid() };
            _mockUserManager.Setup(m => m.GetByUsername("user1")).Returns(currentUser);

            _client.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Test", "user1");

            var response = await _client.DeleteAsync("/api/account?username=targetuser");


            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            var msg = await response.Content.ReadAsStringAsync();
            Assert.Contains("Only admins can delete users", msg);
        }

        [Fact]
        public async Task DeleteUser_ReturnsOk_WhenAdminDeletesSuccessfully()
        {

            var adminUser = new Userdto { Username = "admin", Role = Role.Admin, Id = Guid.NewGuid() };
            _mockUserManager.Setup(m => m.GetByUsername("admin")).Returns(adminUser);

            _mockUserManager.Setup(m => m.Delete("targetuser")).Returns("User deleted successfully");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", "admin"); // Fake token with admin

            var response = await _client.DeleteAsync("/api/account?username=targetuser");


            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var msg = await response.Content.ReadAsStringAsync();
            Assert.Contains("User deleted successfully", msg);
        }


    }
}
