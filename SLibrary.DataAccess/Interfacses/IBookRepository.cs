using System.Collections.Generic;
using SLibrary.DataAccess.Models;


namespace SLibrary.DataAccess.Interfacses
{
    public interface IBookRepository
    {
        List<Book> Load();
        void Add(Book b);
        Book GetByName(string title);
        int BookCount();
        List<Book> GetBooks();
        void UpdateCounts(int bookId, int available, int reserved);
    }

}
