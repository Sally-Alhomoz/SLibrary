using Microsoft.Extensions.Logging;
using SLibrary.DataAccess.Repositories;
using SLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;
using SLibrary.DataAccess.Models;

namespace SLibraryTesting.IntegerationTesting
{
    public class DBBookRepoTest
    {
        private readonly SLibararyDBContext _db;
        private readonly DBBookRepository _repo;
        private readonly ILogger<DBBookRepository> _logger;

        public DBBookRepoTest()
        {
            var options = new DbContextOptionsBuilder<SLibararyDBContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _db = new SLibararyDBContext(options);


            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<DBBookRepository>();

            _repo = new DBBookRepository(_db, _logger);

            _db.Books.Add(new Book { Title = "Test Book", Author = "Author1", Available = 1, Reserved = 0 });
            _db.SaveChanges();
        }

        [Fact]
        public void Add_ShouldIncrementAvailabile_whenexist()
        {
            var newBook = new Book { Title = "New Book", Author = "Author2", Available = 1 };

            _repo.Add(newBook);
            _db.SaveChanges();

            Assert.Equal(2, _repo.BookCount());
        }

        [Fact]
        public void Add_WhenNewBook()
        {
            var book = new Book { Title = "Book A", Author = "Author A" };

            _repo.Add(book);
            _db.SaveChanges();

            var result = _db.Books.FirstOrDefault(b => b.Title == "Book A");
            Assert.NotNull(result);
            Assert.Equal(1, result.Available);
            Assert.Equal(0, result.Reserved);
        }

        [Fact]
        public void GetByName_ShouldReturnBook_WhenExist()
        {
            var book = _repo.GetByName("Test Book");
            Assert.NotNull(book);
            Assert.Equal("Author1", book.Author);
        }

        [Fact]
        public void GetByName_WhenBookNotExist_ShouldReturnNull()
        {
            var book = _repo.GetByName("nonexisten");
            Assert.Null(book);
        }


        [Fact]
        public void Delete_ShouldMarkBookAsDeleted_WhenFound()
        {
            var book = new Book { Title = "Book C", Author = "Author C" };
            _db.Books.Add(book);
            _db.SaveChanges();

            var result = _repo.Delete(book.ID);
            _db.SaveChanges();

            Assert.True(result);
            var deletedBook = _db.Books.Find(book.ID);
            Assert.True(deletedBook.isDeleted);

        }

        [Fact]
        public void Delete_ShouldReturnFalse_WhenBookNotFound()
        {
            var result = _repo.Delete(999);
            _db.SaveChanges();

            Assert.False(result);
        }

        [Fact]
        public void UpdateCounts()
        {
            var book = new Book { Title = "Book D", Author = "Author D", Available = 3, Reserved = 1 };
            _db.Books.Add(book);
            _db.SaveChanges();

            _repo.UpdateCounts(book.ID, 5, 2);
            _db.SaveChanges();

            var updatedBook = _db.Books.Find(book.ID);
            Assert.Equal(5, updatedBook.Available);
            Assert.Equal(2, updatedBook.Reserved);
        }
    }
}
