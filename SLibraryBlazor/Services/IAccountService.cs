using Shared;

namespace SLibraryBlazor.Services
{
    public interface IAccountService
    {
        Task<bool> Login(dtoLogin model);
        Task<string?> GetJwtToken();
        Task Logout();
    }
}
