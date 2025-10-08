using Shared;
using SLibrary.DataAccess.Models;
using System.Collections.Generic;

public interface IBookManager
{
    void Add(CreateBookdto b);
    string ToString();
    List<Bookdto> GetAllBooks();

    string Delete(int id);

}