using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SLibrary.DataAccess;
using Shared;
using Microsoft.EntityFrameworkCore;

namespace SLibrary.Business
{
    public interface IBookManager
    {
        void Add(Book b);
        string ReserveBook(string title, string clientName);
        string ReleaseBook(string title, string clientName);
        string ToString();

    }
    public class BookManager : IBookManager
    {
        IBookRepository bookRepo;
        IReservationRepository reservationRepo;
        private int clientId = 1;

        private readonly SLibararyDBContext _db;

        public BookManager(SLibararyDBContext db)
        {
            _db = db;
        }
        public BookManager()
        {
            bookRepo =RepositoryFactory.CreateBookRepository();
            reservationRepo = RepositoryFactory.CreateReservationRepository();
        }
        public void Add(Book b)
        {
            bookRepo.Add(b);

        }

        public string ReserveBook(string title , string clientName)
        {
            Book temp = bookRepo.GetByName(title);
            if (temp != null)
            {
                if (temp.Available > 0)
                {
                    bookRepo.UpdateCounts(temp.ID, temp.Available - 1, temp.Reserved + 1);

                    Reservation r = new Reservation(clientName, temp.ID, temp.Title, DateTime.Now);
                    reservationRepo.Add(r);
                    return "Book Reserved Successfully !!\n";
                }
                else
                    return "Book Can Not be Reserved\n";
            }
            return "Book Not Found\n";
        }

        public string ReleaseBook(string title, string clientName)
        {
            Book temp = bookRepo.GetByName(title);
            if (temp != null)
            {
                if (temp.Reserved > 0)
                {
                    Reservation res = reservationRepo.GetActiveReservation(title, clientName);

                    if (res == null)
                        return "No active reservation found !!\n";

                    bookRepo.UpdateCounts(temp.ID, temp.Available + 1, temp.Reserved - 1);

                    res.ReleaseDate = DateTime.Now;
                    reservationRepo.Update(res);
                    return "Book Released Successfully !!\n";
                }
                else
                    return "There is NO Book to Release\n";
            }
            return "Book Not Found\n";
        }

        public override string ToString()
        {
            if (bookRepo.BookCount() == 0)
                return "No books available.";

            List<Book> books = bookRepo.GetList();

            StringBuilder sb = new StringBuilder();
            foreach (Book b in books)
            {
                sb.AppendLine($"ID: {b.ID}, Title: {b.Title},Author: {b.Author}, Reserved: {b.Reserved}, Available: {b.Available}");
            }
            return sb.ToString();

        }
        public List<Book> GetAllBooks()
        {
            return bookRepo.GetList();
        }

        public List<Reservation> GetAllReservations()
        {
            return reservationRepo.GetReservation();
        }

        public async Task<List<Book>> GetBooks()
        {
            var books = await _db.Books.ToListAsync();
            return books;
        }

        public async Task<List<Reservation>> GetReservations()
        {
            var reservations = await _db.Reservations.ToListAsync();
            return reservations;
        }

        public async Task<bool> Reserve(int bookid, string clientname)
        {
            var book = await _db.Books.FindAsync(bookid);

            if (book == null)
                return false;

            if (book.Available == 0)
                return false;

            var reservation = new Reservation
            {
                BookID = bookid,
                ClientName = clientname,
                BookTitle = book.Title,
                ReservedDate = DateTime.Now,
                ReleaseDate = null
            };

            _db.Reservations.Add(reservation);

            book.Available = book.Available - 1;
            book.Reserved = book.Reserved + 1;

             _db.SaveChanges();

            return true;
        }

        public async Task<bool> Release(int bookid, string clientname)
        {
            var book = await _db.Books.FindAsync(bookid);

            if (book == null)
                return false;

            if (book.Reserved == 0)
                return false;

            var reservation = await _db.Reservations.Where(r => r.BookID == bookid && r.ClientName == clientname && r.ReleaseDate == null).FirstOrDefaultAsync();

            if (reservation == null)
                return false;

            reservation.ReleaseDate = DateTime.Now;

            book.Available = book.Available + 1;
            book.Reserved = book.Reserved - 1;
            _db.SaveChanges();

            return true;

        }

        public async Task<Reservation> GetReservationById(int id)
        {
            var reservation = await _db.Reservations.FindAsync(id);

            if (reservation == null)
                return null;
            return reservation;
        }

        public async Task Addbook(string name , string author)
        {
            var b = await _db.Books.Where(x => x.Title == name && x.Author == author).FirstOrDefaultAsync();

            if (b != null)
            {
                b.Available = b.Available + 1;

            }
            else
            {
                Book book = new Book
                {
                    Title = name,
                    Author = author,
                    Available = 1,
                    Reserved = 0,
                };
                _db.Books.Add(book);
            }

            _db.SaveChanges();

        }
    }
}
