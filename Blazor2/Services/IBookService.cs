using Shared;

namespace Blazor2.Services
{
    public interface IBookService
    {
        Task<List<Bookdto>> Read();
        Task Delete(int id);
        Task Create(Bookdto book);
        Task<bool> Reserve(string title, string clientname, string phoneNo, string address);
    }
}
