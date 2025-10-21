using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;

namespace SLibrary.DataAccess.Repositories
{
    public class DBBookRepository : IBookRepository
    {
        private readonly SLibararyDBContext _db;
        private readonly ILogger<DBBookRepository> _logger;
        public DBBookRepository(SLibararyDBContext db, ILogger<DBBookRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public List<Book> Load()
        {
            List<Book> books = _db.Books.Where(x => x.isDeleted == false).ToList();
            return books;
        }
        public void Add(Book b)
        {
            _logger.LogInformation("Adding a book");
            var book = _db.Books.Where(x => x.Title == b.Title && x.Author == b.Author).FirstOrDefault();

            if (book != null)
            {
                book.Available = book.Available + 1;
                _logger.LogInformation("Book with title: {title} already exists, increased availability", book.Title);
            }
            else
            {
                Book newbook = new Book
                {
                    Title =b.Title,
                    Author = b.Author,
                    Available = 1,
                    Reserved = 0,
                };
                _db.Books.Add(newbook);
                _logger.LogInformation("Book with title: {title} added successfully", newbook.Title);
            }
        }

        public Book GetByName(string title)
        {
            _logger.LogInformation("Retreving a book with title : {title}", title);
            var b = _db.Books.FirstOrDefault(x => x.Title == title);
            if(b == null)
            {
                _logger.LogWarning("Book not found");
            }
            return b;
        }

        public int BookCount()
        {
            int count = _db.Books.Count();
            return count;
        }

        public List<Book> GetBooks()
        {
            _logger.LogInformation("Retrieving books from the database.");
            List<Book> books = _db.Books.Where(x => x.isDeleted == false).ToList();
            return books;
        }


        public void UpdateCounts(int bookId, int available, int reserved)
        {
            _logger.LogInformation("Updating book counts");
            var book = _db.Books.Find(bookId);

            if(book != null)
            {
                book.Available = available;
                book.Reserved = reserved;
                _logger.LogInformation("Book updated successfully");
            }
            _logger.LogWarning("Book not found");
        }

        public bool Delete(int id)
        {
            _logger.LogInformation("Deleting a book with id : {id}", id);
            var book = _db.Books.Find(id);

            if (book == null)
            {
                _logger.LogWarning("Book with id : {id} not found", id);
                return false;
            }

            book.isDeleted = true;
            _logger.LogInformation("Book with id : {id} deleted succcessfully",id);
            return true;
        }

        public Book GetById(int id)
        {
            _logger.LogInformation("Retrieving book with id : {id}",id);
            var book = _db.Books.Find(id);
            return book;
        }
    }
}
