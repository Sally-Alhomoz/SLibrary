using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shared;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;
using Microsoft.Extensions.Logging;

namespace SLibrary.Business.Managers
{
    public class BookManager : IBookManager
    {
        IBookRepository bookRepo;
        IReservationRepository reservationRepo;
        private readonly ILogger<BookManager> _logger;

        public BookManager(IBookRepository repo , IReservationRepository Rrepo, ILogger<BookManager> logger)
        {
            bookRepo = repo;
            reservationRepo = Rrepo;
            _logger = logger;
        }
        public void Add(CreateBookdto b)
        {
            _logger.LogInformation("Adding a book");
            Book book = new Book
            {
                Title = b.Title,
                Author = b.Author,
                Reserved = 0,
                Available = 1,
            };
            bookRepo.Add(book);
            _logger.LogInformation("Book added to the repository");
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
            _logger.LogInformation("Retrieving books from the database.");
            return bookRepo.GetBooks().Select(b => new Bookdto
            {
                ID = b.ID,
                Title = b.Title,
                Author = b.Author,
                Available = b.Available,
                Reserved = b.Reserved
            }).ToList();
        }

        public string Delete(int id)
        {
            _logger.LogInformation("Deleting a book with id {id}", id);
            var book = bookRepo.GetById(id);
            if (book == null)
            {
                _logger.LogWarning("Book not found");
                return "Book not found";
            }

            var hasReservation = reservationRepo.GetReservations().Any(r => r.BookID == id && r.ReleaseDate ==null);

            if (hasReservation)
            {
                _logger.LogWarning("Book has active reservations");
                return "Book cannot be deleted — it has reservations.";
            }

            var success = bookRepo.Delete(id);
            if (success)
            {
                _logger.LogInformation("Book deleted successfully");
                return "Book deleted successfully";
            }
            _logger.LogWarning("Failed to delete book");
            return "Failed to delete book";

        }

    }
}
