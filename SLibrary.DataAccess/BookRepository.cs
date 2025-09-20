using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Shared;
using System.IO.Ports;

namespace SLibrary.DataAccess
{
    public interface IBookRepository
    {
        void SaveBooksToFile();

        void LoadBooksFromFile();
        void SaveReservationsToFile();

        void LoadReservationsFromFile();
    }
    public class BookRepository : IBookRepository
    {
        static string filePath = "BooksData.csv";
        static string filePath2 = "ReservationsData.csv";
        private List<Book> books = new List<Book>();
        private List<Reservation> reservations = new List<Reservation>();
        public BookRepository()
        {
           LoadBooksFromFile();
            LoadReservationsFromFile();
           
        }
        public void SaveBooksToFile()
        {

            List<string> lines = new List<string>();
            lines.Add($"ID,Title,Author,Available,Reserved");

            foreach (Book b in books)
            {
                lines.Add($"{b.ID},{b.Title},{b.Author},{b.AvailableCount},{b.ReservedCount}");
            }
            File.WriteAllLines(filePath, lines);
        }

        public void LoadBooksFromFile()
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines.Skip(1))
                {
                    var parts = line.Split(',');
                    if (parts.Length < 5)
                        continue;
                    Book b = new Book(int.Parse(parts[0]), parts[1], parts[2], int.Parse(parts[3]), int.Parse(parts[4]));
                    books.Add(b);
                }
            }
        }

        public void SaveReservationsToFile()
        {

            List<string> lines = new List<string>();
            lines.Add($"ID,Client_Name,Book_ID,Book_Title,Reserved_Date,Released_Date");

            foreach (Reservation r in reservations)
            {
                string released = r.ReleaseDate.HasValue ? r.ReleaseDate.Value.ToString() : "-";
                lines.Add($"{r.ClientID},{r.ClientName},{r.BookID},{r.BookTitle},{r.ReservedDate},{released}");
            }
            File.WriteAllLines(filePath2, lines);
        }

        public void LoadReservationsFromFile()
        {
            if (File.Exists(filePath2))
            {
                string[] lines = File.ReadAllLines(filePath2);
                foreach (string line in lines.Skip(1))
                {
                    var parts = line.Split(',');
                    if (parts.Length < 5)
                        continue;
                    DateTime? releaseDate = parts[5] == "-" ? (DateTime?)null : DateTime.Parse(parts[5]);
                    Reservation r = new Reservation(int.Parse(parts[0]), parts[1], int.Parse(parts[2]),parts[3], DateTime.Parse(parts[4]));
                    r.ReleaseDate = releaseDate;
                    reservations.Add(r);
                }
            }
        }


        public void Add(Book b)
        {
            bool found = false;
            foreach (Book b2 in books)
            {
                if (b.Title == b2.Title)
                {
                    found = true;
                    b2.AvailableCount++;
                    b.ID = b2.ID;
                    break;
                }
            }
            if (found != true)
            {
                if (books.Count > 0)
                {
                    var maxId = books.Max(i => i.ID);
                    b.ID = maxId + 1;
                }
                else
                {
                    b.ID = 1;
                }
                books.Add(b);
            }
            SaveBooksToFile();

        }
        public void AddReservation(Reservation r)
        {
            reservations.Add(r);
            SaveReservationsToFile();
        }

        public Book GetByName(string title)
        {
            foreach (Book b in books)
            {
                if (b.Title == title)
                    return b;
            }
            return null;
        }

        public int BookCount()
        {
            return books.Count();
        }

        public List<Book> GetList()
        {
            return books;
        }

        public List<Reservation> GetReservation()
        {
            return reservations;
        }
    }
}
