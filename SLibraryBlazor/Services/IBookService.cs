using Shared;

namespace SLibraryBlazor.Services
{
    public interface IBookService
    {
        Task<List<Bookdto>> Read();
        Task Delete(int id);
        Task Create(Bookdto book);
        Task<bool> Reserve(string title, string clientname, string phoneNo, string address);
    }
}
