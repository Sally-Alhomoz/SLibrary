using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace SLibrary.DataAccess
{
    internal class FileBookRepository : IBookRepository
    {
        private string filePath;
        public FileBookRepository(string filepath)
        {
            filePath = filepath;
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
            List<Book> list = Load();
            foreach(Book book in list)
            {
                if(book.Title == b.Title && book.Author == b.Author)
                {
                    found = true;
                    book.AvailableCount++;
                    b.ID = book.ID;
                    break;
                }
            }
            if(!found == true)
            {
                if (list.Count > 0)
                {
                    var maxId = list.Max(i => i.ID);
                    b.ID = maxId + 1;
                }
                else
                {
                    b.ID = 1;
                }
                list.Add(b);
            }
            Save(list);
        }
        public Book GetByName(string title)
        {
            List<Book> list = Load();
            foreach(Book b in list)
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
        public List<Book> GetList()
        {
            return Load();
        }
        public void UpdateCounts(int bookId, int available, int reserved)
        {
            List<Book> list = Load();

            Book found = list.FirstOrDefault(x => x.ID == bookId);

            if(found != null)
            {
                found.AvailableCount = available;
                found.ReservedCount = reserved;
                Save(list);
            }
        }
    }

}
