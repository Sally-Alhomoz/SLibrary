using Shared;
using SLibrary.DataAccess.Models;
using System.Collections.Generic;

public interface IBookManager
{
    void Add(CreateBookdto b);
    string ToString();
    List<Bookdto> GetAllBooks();

    string Delete(int id);
    (List<Bookdto> books, int totalCount) GetUsersPaged(int page,
    int pageSize,
    string search,
    string sortBy,
    string sortDirection);

}