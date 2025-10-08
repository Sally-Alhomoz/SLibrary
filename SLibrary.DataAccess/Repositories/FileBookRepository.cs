using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;

namespace SLibrary.DataAccess.Repositories
{
    public class FileBookRepository : IBookRepository
    {
        private string filePath;
        private List<Book> books;
        public FileBookRepository(string filepath)
        {
            filePath = filepath;
            books = Load();
        }

        public List<Book> Load()
        {
            if (!File.Exists(filePath))
                return new List<Book>();

            var json = File.ReadAllText(filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new List<Book>();
            return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }

        private void Save(List<Book> list)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            File.WriteAllText(filePath, JsonSerializer.Serialize(list, options));
        }
        public void Add(Book b)
        {
            bool found = false;
            foreach (Book book in books)
            {
                if (book.Title == b.Title && book.Author == b.Author)
                {
                    found = true;
                    book.Available++;
                    b.ID = book.ID;
                    break;
                }
            }
            if (!found == true)
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
            Save(books);
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
            return Load().Count;
        }
        public List<Book> GetBooks()
        {
            return Load();
        }
        public void UpdateCounts(int bookId, int available, int reserved)
        {

            Book found = books.FirstOrDefault(x => x.ID == bookId);

            if (found != null)
            {
                found.Available = available;
                found.Reserved = reserved;
                Save(books);
            }
        }

        public bool Delete(int id)
        {
            var book = books.FirstOrDefault(b => b.ID == id);
            if (book == null)
                return false;

            books.Remove(book);
            Save(books);
            return true;
        }

       public Book GetById(int id)
        {
            foreach (Book b in books)
            {
                if (b.ID == id)
                    return b;
            }
            return null;
        }
    }

}
