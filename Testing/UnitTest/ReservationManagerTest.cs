using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using SLibrary.Business.Managers;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;
using SLibrary.DataAccess.SUnitOfWork;
using Shared;

namespace SLibraryTests.UnitTest
{
    public class ReservationManagerTest
    {
        private readonly Mock<IUnitOfWork> mockUow;
        private readonly Mock<ILogger<ReservationManager>> mockLogger;
        private readonly Mock<IBookRepository> mockBookRepo;
        private readonly Mock<IReservationRepository> mockReservationRepo;
        private readonly ReservationManager manager;

        public ReservationManagerTest()
        {
            mockUow = new Mock<IUnitOfWork>();
            mockLogger = new Mock<ILogger<ReservationManager>>();
            mockBookRepo = new Mock<IBookRepository>();
            mockReservationRepo = new Mock<IReservationRepository>();

            mockUow.SetupGet(u => u.DBBooks).Returns(mockBookRepo.Object);
            mockUow.SetupGet(u => u.DBReservations).Returns(mockReservationRepo.Object);

            manager = new ReservationManager(mockUow.Object, mockLogger.Object);
        }

        [Fact]
        public void ReserveBook_ShouldReturnSuccessMsg_WhenAvailable()
        {
            var book = new Book { ID = 1, Title = "TestBook", Available = 2, Reserved = 0 };
            mockBookRepo.Setup(r => r.GetByName("TestBook")).Returns(book);

            var result = manager.ReserveBook("TestBook", "Client1");

            Assert.Equal("Book Reserved Successfully !!\n", result);
            mockBookRepo.Verify(r => r.UpdateCounts(book.ID, 1, 1), Times.Once);
            mockReservationRepo.Verify(r => r.Add(It.IsAny<Reservation>()), Times.Once);
            mockUow.Verify(u => u.Complete(), Times.Once);
        }

        [Fact]
        public void ReserveBook_ShouldReturnMsg_WhenCannotReserve()
        {
            var book = new Book { ID = 1, Title = "TestBook", Available = 0, Reserved = 1 };
            mockBookRepo.Setup(r => r.GetByName("TestBook")).Returns(book);

            var result = manager.ReserveBook("TestBook", "Client1");

            Assert.Equal("Book Can Not be Reserved\n", result);
            mockBookRepo.Verify(r => r.UpdateCounts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            mockReservationRepo.Verify(r => r.Add(It.IsAny<Reservation>()), Times.Never);
            mockUow.Verify(u => u.Complete(), Times.Never);
        }

        [Fact]
        public void ReserveBook_ShouldReturnMsg_WhenBookDeleted()
        {
            var book = new Book { ID = 1, Title = "DeletedBook", isDeleted = true };
            mockBookRepo.Setup(r => r.GetByName("DeletedBook")).Returns(book);

            var result = manager.ReserveBook("DeletedBook", "Client1");

            Assert.Equal("Book Not Found - Deleted\n", result);
        }

        [Fact]
        public void ReserveBook_ShouldReturnMsg_WhenBookNull()
        {
            mockBookRepo.Setup(r => r.GetByName("MissingBook")).Returns((Book)null);

            var result = manager.ReserveBook("MissingBook", "Client1");

            Assert.Equal("Book Not Found\n", result);
        }


        [Fact]
        public void ReleaseBook_ShouldReturnMsg_WhenBookReserved()
        {

            var book = new Book { ID = 1, Title = "TestBook", Reserved = 1, Available = 1 };
            var reservation = new Reservation { BookID = 1, BookTitle = "TestBook", ClientName = "Client1" };
            mockBookRepo.Setup(r => r.GetByName("TestBook")).Returns(book);
            mockReservationRepo.Setup(r => r.GetActiveReservation("TestBook", "Client1")).Returns(reservation);

            var result = manager.ReleaseBook("TestBook", "Client1");

            Assert.Equal("Book Released Successfully !!\n", result);
            mockBookRepo.Verify(r => r.UpdateCounts(book.ID, 2, 0), Times.Once);
            mockReservationRepo.Verify(r => r.Update(reservation), Times.Once);
            mockUow.Verify(u => u.Complete(), Times.Once);
        }

        [Fact]
        public void ReleaseBook_ShouldReturnMsg_NoActiveReservation()
        {
            var book = new Book { ID = 1, Title = "TestBook", Reserved = 1, Available = 1 };
            mockBookRepo.Setup(r => r.GetByName("TestBook")).Returns(book);
            mockReservationRepo.Setup(r => r.GetActiveReservation("TestBook", "Client1")).Returns((Reservation)null);

            var result = manager.ReleaseBook("TestBook", "Client1");

            Assert.Equal("No active reservation found !!\n", result);
            mockBookRepo.Verify(r => r.UpdateCounts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            mockReservationRepo.Verify(r => r.Update(It.IsAny<Reservation>()), Times.Never);
            mockUow.Verify(u => u.Complete(), Times.Never);
        }

        [Fact]
        public void ReleaseBook_ShouldReturnMsg_WhenReservedZero()
        {
            var book = new Book { ID = 1, Title = "TestBook", Reserved = 0, Available = 1 };
            mockBookRepo.Setup(r => r.GetByName("TestBook")).Returns(book);

            var result = manager.ReleaseBook("TestBook", "Client1");

            Assert.Equal("There is NO Book to Release\n", result);
        }

        [Fact]
        public void ReleaseBook_ShouldReturnMsg_WhenBookNull()
        {
            mockBookRepo.Setup(r => r.GetByName("MissingBook")).Returns((Book)null);

            var result = manager.ReleaseBook("MissingBook", "Client1");

            Assert.Equal("Book Not Found\n", result);
        }


        [Fact]
        public void GetAllReservations_ShouldReturnAllReservations()
        {
            var reservations = new List<Reservation>
            {
                new Reservation { ID = 1, BookID = 1, BookTitle = "Book1", ClientName = "Client1", ReservedDate = DateTime.Now },
                new Reservation { ID = 2, BookID = 2, BookTitle = "Book2", ClientName = "Client2", ReservedDate = DateTime.Now }
            };
            mockReservationRepo.Setup(r => r.GetReservations()).Returns(reservations);

            var result = manager.GetAllReservations();

            Assert.Equal(2, result.Count);
            Assert.Equal("Book1", result[0].BookTitle);
        }

        [Fact]
        public void GetReservationById_ShouldReturnReservation()
        {
            var reservation = new Reservation { ID = 1, BookID = 1, BookTitle = "Book1", ClientName = "Client1", ReservedDate = DateTime.Now };
            mockReservationRepo.Setup(r => r.GetReservationById(1)).Returns(reservation);

            var result = manager.GetReservationById(1);

            Assert.Equal(1, result.ID);
            Assert.Equal("Book1", result.BookTitle);
        }
    }
}
