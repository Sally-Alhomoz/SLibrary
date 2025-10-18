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

namespace SLibraryTesting.APITesting
{
    public class ReservationAPITest : IClassFixture<WebApplicationFactory<ReservationsController>>
    {
        private readonly HttpClient _client;
        private readonly Mock<IReservationManager> _mockresManager;

        public ReservationAPITest(WebApplicationFactory<ReservationsController> factory)
        {
            _mockresManager = new Mock<IReservationManager>();

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

                    services.AddSingleton(_mockresManager.Object);
                });
            }).CreateClient();
        }

        [Fact]
        public async Task ReadRservationss_ShouldReturnOk()
        {
            var reservations = new List<Reservationdto>
            {
                new Reservationdto {ID = 1 , BookID =1 , BookTitle="Book1", ClientName="Client1" , ReservedDate=DateTime.Now},
                new Reservationdto {ID = 2 , BookID =2 , BookTitle="Book2", ClientName="Client2" , ReservedDate=DateTime.Now}
            };
            _mockresManager.Setup(m => m.GetAllReservations()).Returns(reservations);

            var response = await _client.GetAsync("/api/reservations");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<List<Reservationdto>>();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetReservationById_ShouldReturnOk_WhenFound()
        {
            var res = new Reservationdto { ID = 3, BookID = 3, BookTitle = "Book3", ClientName = "Client3", ReservedDate = DateTime.Now };

            _mockresManager.Setup(m => m.GetReservationById(3)).Returns(res);

            var response = await _client.GetAsync("/api/reservations/GetByID?Id=3");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<Reservationdto>();
            Assert.NotNull(result);
            Assert.Equal(3, result.ID);
        }

        [Fact]
        public async Task GetReservationById_ShouldReturnNotFound_WhenNotFound()
        {
            _mockresManager.Setup(m => m.GetReservationById(99)).Returns((Reservationdto)null);

            var response = await _client.GetAsync("/api/reservations/GetByID?Id=99");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Fact]
        public async Task CreateReservation_ReturnsOk_WhenAuthorized()
        {
            _mockresManager.Setup(m => m.ReserveBook(It.IsAny<string>(), It.IsAny<string>())).Returns("Successfully reserved");

            var response = await _client.PostAsJsonAsync("/api/reservations/Reserve?title=Book2&clientname=Client2", new { });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var msg = await response.Content.ReadAsStringAsync();
            Assert.Contains("Book reserved successfully", msg);
        }

        [Fact]
        public async Task CreateReservation_RetrunBadRequest_WhenBookDeleted()
        {
            _mockresManager.Setup(m => m.ReserveBook(It.IsAny<string>(), It.IsAny<string>())).Returns("Deleted from system");

            var response = await _client.PostAsJsonAsync("/api/reservations/Reserve?title=BookB&clientname=ClientB", new { });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var msg = await response.Content.ReadAsStringAsync();
            Assert.Contains("Book Can Not be reserved - Deleted", msg);
        }

        [Fact]
        public async Task CreateReservation_RetrunBadRequest_WhenFailure()
        {
            _mockresManager.Setup(m => m.ReserveBook(It.IsAny<string>(), It.IsAny<string>())).Returns("Fialed");

            var response = await _client.PostAsJsonAsync("/api/reservations/Reserve?title=BookB&clientname=ClientB", new { });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var msg = await response.Content.ReadAsStringAsync();
            Assert.Contains("Book Can Not be reserved", msg);
        }

        [Fact]
        public async Task DeleteReservation_ReturnsOk_WhenSuccess()
        {
            _mockresManager.Setup(m => m.ReleaseBook(It.IsAny<string>(), It.IsAny<string>())).Returns("Successfully");

            var response = await _client.DeleteAsync("/api/reservations/Release?title=Book1&clientname=Client1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var msg = await response.Content.ReadAsStringAsync();
            Assert.Contains("Book released successfully", msg);
        }

        [Fact]
        public async Task DeleteReservation_ReturnsBadRequest_WhenFailure()
        {
            _mockresManager.Setup(m => m.ReleaseBook(It.IsAny<string>(), It.IsAny<string>())).Returns("Failed");

            var response = await _client.DeleteAsync("/api/reservations/Release?title=BookB&clientname=ClientB");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var msg = await response.Content.ReadAsStringAsync();
            Assert.Contains("Book Can Not be released", msg);
        }


    }
}
