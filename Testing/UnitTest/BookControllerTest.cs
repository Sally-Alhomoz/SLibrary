using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shared;
using SLibrary.Business.Interfaces;
using SLibraryAPI.Controllers;

namespace SLibraryTests.UnitTest
{
    public class BookControllerTest
    {
        private readonly Mock<IBookManager> mockBookManager;
        private readonly Mock<ILogger<BooksController>> mockLogger;
        private readonly BooksController controller;

        public BookControllerTest()
        {
            mockBookManager = new Mock<IBookManager>();
            mockLogger = new Mock<ILogger<BooksController>>();
            controller = new BooksController(mockBookManager.Object, mockLogger.Object);
        }


        [Fact]
        public void Read()
        {
            var books = new List<Bookdto> { new Bookdto { ID = 1, Title = "Book1" } };
            mockBookManager.Setup(m => m.GetAllBooks()).Returns(books);

            var result = controller.Read().Result as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(books, result.Value);
            mockBookManager.Verify(m => m.GetAllBooks(), Times.Once);
        }

        [Fact]
        public void Create()
        {
            var result = controller.Create("Title1", "Author1").Result as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Book added successfully", result.Value);
            mockBookManager.Verify(m => m.Add(It.Is<CreateBookdto>(d => d.Title == "Title1" && d.Author == "Author1")), Times.Once);
        }

        [Fact]
        public void Delete_WhenSuccess()
        {
            mockBookManager.Setup(m => m.Delete(1)).Returns("Book deleted successfully");

            var result = controller.Delete(1).Result as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Book deleted successfully", result.Value);
        }

        [Fact]
        public void Delete_ReturnsBadRequest_WhenFail()
        {
            mockBookManager.Setup(m => m.Delete(1)).Returns("Failed to delete book");

            var result = controller.Delete(1).Result as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Failed to delete book", result.Value);
        }
    }
}
