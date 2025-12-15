using Shared;

namespace SLibrary.Blazor.Services.Book_Services
{
    public interface IBookService
    {
        Task<(List<Bookdto> Items, int TotalCount)> Read(int page, int pageSize, string search, string sortBy, string sortDirection);
        Task Delete(int id);
        Task Create(Bookdto book);
        Task<bool> Reserve(string title, string clientname, string phoneNo, string address);
    }
}
