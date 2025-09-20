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
    public class BookManager
    {
        BookRepository repo = new BookRepository();
        private int clientId = 1;  

        public BookManager()
        {

        }
        public void Add(Book b)
        {
            repo.Add(b);

        }

        public string ReserveBook(string title , string clientName)
        {
            Book temp = repo.GetByName(title);
            if (temp != null)
            {
                if (temp.AvailableCount > 0)
                {
                    temp.ReservedCount++;
                    temp.AvailableCount--;
                    int id = clientId++;
                    Reservation r = new Reservation(id, clientName, temp.ID, temp.Title, DateTime.Now);
                    repo.SaveBooksToFile();
                    repo.AddReservation(r);
                    repo.SaveReservationsToFile();
                    return "Book Reserved Successfully !!\n";
                }
                else
                    return "Book Can Not be Reserved\n";
            }
            return "Book Not Found\n";
        }

        public override string ToString()
        {
            if (repo.BookCount() == 0)
                return "No books available.";

            List<Book> books = repo.GetList();

            StringBuilder sb = new StringBuilder();
            foreach (Book b in books)
            {
                sb.AppendLine($"ID: {b.ID}, Title: {b.Title},Author: {b.Author}, Reserved: {b.ReservedCount}, Available: {b.AvailableCount}");
            }
            return sb.ToString();

        }

        public string ReleaseBook(string title, string clientName)
        {
            Book temp = repo.GetByName(title);
            if (temp != null)
            {
                if (temp.ReservedCount > 0)
                {
                    Reservation res = repo.GetReservation().Where(r => r.BookTitle == title&& r.ClientName == clientName&& r.ReleaseDate == null)
                          .OrderByDescending(r => r.ReservedDate)
                          .FirstOrDefault();
                    if (res == null)
                        return "No active reservation found !!\n";

                    temp.ReservedCount--;
                    temp.AvailableCount++;

                    res.ReleaseDate = DateTime.Now;
                    repo.SaveBooksToFile();
                    repo.SaveReservationsToFile();
                    return "Book Released Successfully !!\n";
                }
                else
                    return "There is NO Book to Release\n";
            }
            return "Book Not Found\n";
        }

        public List<Book> GetAllBooks()
        {
            return repo.GetList();
        }

        public List<Reservation> GetAllReservations()
        {
            return repo.GetReservation();
        }
    }
}
