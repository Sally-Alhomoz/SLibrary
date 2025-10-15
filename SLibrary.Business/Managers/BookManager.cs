﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shared;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;
using Microsoft.Extensions.Logging;
using SLibrary.DataAccess.SUnitOfWork;

namespace SLibrary.Business.Managers
{
    public class BookManager : IBookManager
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<BookManager> _logger;

        public BookManager(IUnitOfWork uow, ILogger<BookManager> logger)
        {
            _uow = uow;
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
            _uow.DBBooks.Add(book);
            _uow.Complete();
            _logger.LogInformation("Book added to the repository");
        }


        public override string ToString()
        {
            if (_uow.DBBooks.BookCount() == 0)
                return "No books available.";

            List<Book> books = _uow.DBBooks.GetBooks();

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
            return _uow.DBBooks.GetBooks().Select(b => new Bookdto
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
            var book = _uow.DBBooks.GetById(id);
            if (book == null)
            {
                _logger.LogWarning("Book not found");
                return "Book not found";
            }

            var hasReservation = _uow.DBReservations.GetReservations().Any(r => r.BookID == id && r.ReleaseDate ==null);

            if (hasReservation)
            {
                _logger.LogWarning("Book has active reservations");
                return "Book cannot be deleted — it has reservations.";
            }

            var success = _uow.DBBooks.Delete(id);
            if (success)
            {
                _uow.Complete();
                _logger.LogInformation("Book deleted successfully");
                return "Book deleted successfully";
            }
            _logger.LogWarning("Failed to delete book");
            return "Failed to delete book";

        }

    }
}
