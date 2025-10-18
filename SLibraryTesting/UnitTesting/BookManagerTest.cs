using Microsoft.Extensions.Logging;
using Moq;
using Shared;
using SLibrary.Business.Managers;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;
using SLibrary.DataAccess.SUnitOfWork;
using Xunit;

namespace SLibraryTesting.UnitTesting
{
    public class BookMnagerTest
    {
        private readonly Mock<IUnitOfWork> mockUow;
        private readonly Mock<ILogger<BookManager>> mockLogger;
        private readonly Mock<IBookRepository> mockBookRepo;
        private readonly Mock<IReservationRepository> mockReservationRepo;
        private readonly BookManager manager;
        public BookMnagerTest()
        {
            mockUow = new Mock<IUnitOfWork>();
            mockLogger = new Mock<ILogger<BookManager>>();
            mockBookRepo = new Mock<IBookRepository>();
            mockReservationRepo = new Mock<IReservationRepository>();

            mockUow.SetupGet(u => u.DBBooks).Returns(mockBookRepo.Object);
            mockUow.SetupGet(u => u.DBReservations).Returns(mockReservationRepo.Object);

            manager = new BookManager(mockUow.Object, mockLogger.Object);
        }

        [Fact]
        public void AddBookTest()
        {

            var dto = new CreateBookdto { Title = "Test", Author = "Author" };

            manager.Add(dto);

            mockBookRepo.Verify(r => r.Add(It.IsAny<Book>()), Times.Once);
            mockUow.Verify(u => u.Complete(), Times.Once);

        }


        [Fact]
        public void Delete_ShouldReturnNotFoundMsg_BookNotFound()
        {
            int bookId = 99;
            mockBookRepo.Setup(r => r.GetById(bookId)).Returns((Book)null);

            var result = manager.Delete(bookId);

            Assert.Equal("Book not found", result);
            mockBookRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
            mockUow.Verify(u => u.Complete(), Times.Never);
        }


        [Fact]
        public void Delete_BookHasActiveReservation_ReturnsCannotDeleteMessage()
        {
            int bookId = 1;
            mockBookRepo.Setup(r => r.GetById(bookId)).Returns(new Book { ID = bookId, Title = "Reserved Book" });

            var reservations = new List<Reservation>
            {
                new Reservation { BookID = bookId, ReleaseDate = null }
            };
            mockReservationRepo.Setup(r => r.GetReservations()).Returns(reservations);

            var result = manager.Delete(bookId);

            Assert.Equal("Book cannot be deleted — it has reservations.", result);
            mockBookRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
            mockUow.Verify(u => u.Complete(), Times.Never);
        }

        [Fact]
        public void Delete_RepositoryFailsToDelete()
        {
            int bookId = 1;
            mockBookRepo.Setup(r => r.GetById(bookId)).Returns(new Book { ID = bookId, Title = "Failing Book" });

            mockReservationRepo.Setup(r => r.GetReservations()).Returns(new List<Reservation>());
            mockBookRepo.Setup(r => r.Delete(bookId)).Returns(false);

            var result = manager.Delete(bookId);

            Assert.Equal("Failed to delete book", result);
            mockBookRepo.Verify(r => r.Delete(bookId), Times.Once);
            mockUow.Verify(u => u.Complete(), Times.Never);
        }

        [Fact]
        public void GetAllBooks_ShouldReturnsAllBooks()
        {

            var books = new List<Book>
            {
                new Book { ID = 1, Title = "Book1", Author = "Author1", Available = 1, Reserved = 0 },
                new Book { ID = 2, Title = "Book2", Author = "Author2", Available = 2, Reserved = 1 }
            };
            mockBookRepo.Setup(r => r.GetBooks()).Returns(books);


            var result = manager.GetAllBooks();

            Assert.Equal(2, result.Count);
            Assert.Equal("Book1", result[0].Title);
            Assert.Equal("Book2", result[1].Title);
        }

        [Fact]
        public void ToString_ReturnsAllBooks()
        {
            var books = new List<Book>
            {
               new Book { ID = 1, Title = "Book1", Author = "Author1", Available = 3, Reserved = 1 }
            };
            mockBookRepo.Setup(r => r.GetBooks()).Returns(books);
            mockBookRepo.Setup(r => r.BookCount()).Returns(1);

            var result = manager.ToString();

            Assert.Contains("ID: 1", result);
            Assert.Contains("Book1", result);
            Assert.Contains("Author1", result);
        }
    }
}