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
        void Save();

        void Load();
        void Add(Book b);
        Book GetByName(string title);
        int BookCount();
        List<Book> GetList();
    }
    public class BookRepository : IBookRepository
    {
        static string filePath = "BooksData.csv";
        private List<Book> books = new List<Book>();
        public BookRepository()
        {
           Load();
        }
        public void Save()
        {

            List<string> lines = new List<string>();
            lines.Add($"ID,Title,Author,Available,Reserved");

            foreach (Book b in books)
            {
                lines.Add($"{b.ID},{b.Title},{b.Author},{b.AvailableCount},{b.ReservedCount}");
            }
            File.WriteAllLines(filePath, lines);
        }

        public void Load()
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
            Save();

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
    }
}
