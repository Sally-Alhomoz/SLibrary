using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SLibrary.DataAccess;
using Shared;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;

namespace SLibrary.Business
{
    public class BookManager : IBookManager
    {
        IBookRepository bookRepo;

        public BookManager(IBookRepository repo)
        {
            bookRepo = repo;
        }
        public void Add(CreateBookdto b)
        {

            Book book = new Book
            {
                Title = b.Title,
                Author = b.Author,
                Reserved = 0,
                Available=1,
            };
            bookRepo.Add(book);
        }


        public override string ToString()
        {
            if (bookRepo.BookCount() == 0)
                return "No books available.";

            List<Book> books = bookRepo.GetBooks();

            StringBuilder sb = new StringBuilder();
            foreach (Book b in books)
            {
                sb.AppendLine($"ID: {b.ID}, Title: {b.Title},Author: {b.Author}, Reserved: {b.Reserved}, Available: {b.Available}");
            }
            return sb.ToString();

        }
        public List<Bookdto> GetAllBooks()
        {
            return bookRepo.GetBooks().Select(b => new Bookdto
            {
                ID = b.ID,
                Title = b.Title,
                Author = b.Author,
                Available = b.Available,
                Reserved = b.Reserved
            }).ToList();
        }

    }
}
