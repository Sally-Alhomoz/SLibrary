using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shared;
using SLibrary.Business.Interfaces;
using SLibraryAPI.Controllers;

namespace SLibraryTesting.UnitTesting
{
    public class ReservationControllerTest
    {
        private readonly Mock<IReservationManager> mockReservationManager;
        private readonly Mock<ILogger<ReservationsController>> mockLogger;
        private readonly ReservationsController controller;

        public ReservationControllerTest()
        {
            mockReservationManager = new Mock<IReservationManager>();
            mockLogger = new Mock<ILogger<ReservationsController>>();
            controller = new ReservationsController(mockReservationManager.Object, mockLogger.Object);
        }

        [Fact]
        public void Read()
        {
            var reservations = new List<Reservationdto> { new Reservationdto { ID = 1, BookTitle = "Book1" } };
            mockReservationManager.Setup(m => m.GetAllReservations()).Returns(reservations);

            var result = controller.Read().Result as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(reservations, result.Value);
            mockReservationManager.Verify(m => m.GetAllReservations(), Times.Once);
        }

        [Fact]
        public void GetReservation_WhenExists()
        {
            var reservation = new Reservationdto { ID = 1, BookTitle = "Book1" };
            mockReservationManager.Setup(m => m.GetReservationById(1)).Returns(reservation);

            var result = controller.GetReservation(1).Result as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(reservation, result.Value);
        }

        [Fact]
        public void GetReservation_WhenNotFound()
        {
            mockReservationManager.Setup(m => m.GetReservationById(1)).Returns((Reservationdto)null);

            var result = controller.GetReservation(1).Result as NotFoundResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void Create_whenSuccess()
        {
            mockReservationManager.Setup(m => m.ReserveBook("Book1", "Client1")).Returns("Book Reserved Successfully !!\n");

            var result = controller.Create("Book1", "Client1").Result as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Book reserved successfully", result.Value);
        }

        [Fact]
        public void Create_WhenUnavailable()
        {
            mockReservationManager.Setup(m => m.ReserveBook("Book1", "Client1")).Returns("Book Can Not be Reserved\n");

            var result = controller.Create("Book1", "Client1").Result as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Book Can Not be reserved", result.Value);
        }

        [Fact]
        public void Create_WhenDeleted()
        {
            mockReservationManager.Setup(m => m.ReserveBook("Book1", "Client1")).Returns("Book Not Found - Deleted\n");

            var result = controller.Create("Book1", "Client1").Result as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Book Can Not be reserved - Deleted", result.Value);
        }

        [Fact]
        public void Delete_WhenSuccess()
        {
            mockReservationManager.Setup(m => m.ReleaseBook("Book1", "Client1")).Returns("Book Released Successfully !!\n");

            var result = controller.Delete("Book1", "Client1").Result as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Book released successfully", result.Value);
        }

        [Fact]
        public void Delete_ReturnsBadRequest_WhenFail()
        {
            mockReservationManager.Setup(m => m.ReleaseBook("Book1", "Client1")).Returns("There is NO Book to Release\n");

            var result = controller.Delete("Book1", "Client1").Result as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Book Can Not be released", result.Value);
        }
    }
}
