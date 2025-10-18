using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SLibraryAPI.Controllers;
using Shared;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Authentication;

namespace SLibraryTesting.APITesting
{
    public class BookAPITest : IClassFixture<WebApplicationFactory<BooksController>>
    {
        private readonly HttpClient _client;
        private readonly Mock<IBookManager> _mockBookManager;

        public BookAPITest(WebApplicationFactory<BooksController> factory)
        {
            _mockBookManager = new Mock<IBookManager>();

            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthentication>("Test", options => { });

                    // Replace IBookManager with mock
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IBookManager));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddSingleton(_mockBookManager.Object);
                });
            }).CreateClient();
        }


        [Fact]
        public async Task ReadBooks_ShouldReturnsOk()
        {
            var books = new List<Bookdto>
            {
                new Bookdto { ID = 1, Title = "Book1", Author = "Author1" },
                new Bookdto { ID = 2, Title = "Book2", Author = "Author2" }
            };
            _mockBookManager.Setup(m => m.GetAllBooks()).Returns(books);

            var response = await _client.GetAsync("/api/books");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<List<Bookdto>>();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task CreateBook_ReturnsOk_WhenAuthorized()
        {
            _mockBookManager.Setup(m => m.Add(It.IsAny<CreateBookdto>()));

            var response = await _client.PostAsJsonAsync("/api/books?name=New Book&author=AuthorX", new { });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var msg = await response.Content.ReadAsStringAsync();
            Assert.Contains("Book added successfully", msg);
        }

        [Fact]
        public async Task CreateBook_ShouldReturnBadRequest_WhenMissingParams()
        {
            var response = await _client.PostAsJsonAsync("/api/books", new { });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteBook_ReturnsOk_WhenSuccess()
        {
            _mockBookManager.Setup(m => m.Delete(1)).Returns("Book deleted successfully");

            var response = await _client.DeleteAsync("/api/books?id=1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var msg = await response.Content.ReadAsStringAsync();
            Assert.Contains("Book deleted successfully", msg);
        }

        [Fact]
        public async Task DeleteBook_ShouldReturnBadRequest_WhenFail()
        {
            _mockBookManager.Setup(m => m.Delete(1)).Returns("Failed to delete book");

            var response = await _client.DeleteAsync("/api/books?id=1");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
