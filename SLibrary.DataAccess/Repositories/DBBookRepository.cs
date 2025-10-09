using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;

namespace SLibrary.DataAccess.Repositories
{
    public class DBBookRepository : IBookRepository
    {
        private readonly SLibararyDBContext _db;
        public DBBookRepository(SLibararyDBContext db)
        {
            _db = db;
        }

        public List<Book> Load()
        {
            List<Book> books = _db.Books.ToList();
            return books;
        }
        public void Add(Book b)
        {
            var book = _db.Books.Where(x => x.Title == b.Title && x.Author == b.Author).FirstOrDefault();

            if (book != null)
            {
                book.Available = book.Available + 1;

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
            }

            _db.SaveChanges();
        }

        public Book GetByName(string title)
        {
            var b = _db.Books.FirstOrDefault(x => x.Title == title);
            return b;
        }

        public int BookCount()
        {
            int count = _db.Books.Count();
            return count;
        }

        public List<Book> GetBooks()
        {
            List<Book> books = _db.Books.ToList();
            return books;
        }


        public void UpdateCounts(int bookId, int available, int reserved)
        {
            var book = _db.Books.Find(bookId);

            if(book != null)
            {
                book.Available = available;
                book.Reserved = reserved;
                _db.SaveChanges();
            }
        }

        public bool Delete(int id)
        {
            var book = _db.Books.Find(id);

            if (book == null)
                return false;

            book.isDeleted = true;
            _db.SaveChanges();
            return true;
        }

        public Book GetById(int id)
        {
            var book = _db.Books.Find(id);
            return book;
        }
    }
}
