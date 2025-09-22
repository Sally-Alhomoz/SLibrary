using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SLibrary.DataAccess;
using Shared;


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
        IBookRepository bookRepo = new BookRepository();
        IReservationRepository reservationRepo = new ReservationRepository();
        private int clientId = 1;  

        public BookManager()
        {

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
                if (temp.AvailableCount > 0)
                {
                    temp.ReservedCount++;
                    temp.AvailableCount--;
                    Reservation r = new Reservation(clientId, clientName, temp.ID, temp.Title, DateTime.Now);
                    bookRepo.Save();
                    reservationRepo.Add(r);
                    reservationRepo.Save();
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
                if (temp.ReservedCount > 0)
                {
                    Reservation res = reservationRepo.GetReservation().Where(r => r.BookTitle == title&& r.ClientName == clientName&& r.ReleaseDate == null)
                          .OrderByDescending(r => r.ReservedDate)
                          .FirstOrDefault();
                    if (res == null)
                        return "No active reservation found !!\n";

                    temp.ReservedCount--;
                    temp.AvailableCount++;

                    res.ReleaseDate = DateTime.Now;
                    bookRepo.Save();
                    reservationRepo.Save();
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
                sb.AppendLine($"ID: {b.ID}, Title: {b.Title},Author: {b.Author}, Reserved: {b.ReservedCount}, Available: {b.AvailableCount}");
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
    }
}
