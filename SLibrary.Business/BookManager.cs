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
        List<Book> books = new List<Book>();

        public BookManager()
        {
            books = repo.LoadBooksFromFile();
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
            repo.SaveBooksToFile(books);

        }

        public string ReserveBook(string title)
        {
            foreach (Book b in books)
            {
                if (b.Title == title)
                {
                    if (b.AvailableCount > 0)
                    {
                        b.ReservedCount++;
                        b.AvailableCount--;
                        repo.SaveBooksToFile(books);
                        return "Book Reserved Successfully !!";
                    }
                    else
                        return "Book Can Not be Reserved ";
                }
            }
            return "Book Not Found";
        }

        public override string ToString()
        {
            if (books.Count == 0)
                return "No books available.";

            StringBuilder sb = new StringBuilder();
            foreach (Book b in books)
            {
                sb.AppendLine($"ID: {b.ID}, Title: {b.Title}, Reserved: {b.ReservedCount}, Available: {b.AvailableCount}");
            }
            return sb.ToString();

        }

        public string ReleaseBook(string title)
        {
            foreach (Book b in books)
            {
                if (b.Title == title)
                {
                    if (b.ReservedCount > 0)
                    {
                        b.ReservedCount--;
                        b.AvailableCount++;
                        repo.SaveBooksToFile(books);
                        return "Book Released Successfully !!";
                    }
                    else
                        return "There is NO Book to Release";
                }
            }
            return "Book Not found";
        }
    }
}
