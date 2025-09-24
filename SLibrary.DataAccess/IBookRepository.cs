using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Shared;
using System.IO.Ports;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;


namespace SLibrary.DataAccess
{
    public interface IBookRepository
    {
        List<Book> Load();
        void Add(Book b);
        Book GetByName(string title);
        int BookCount();
        List<Book> GetList();
        void UpdateCounts(int bookId, int available, int reserved);
    }
   
}
