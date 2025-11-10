

using Shared;

namespace Blazor2.Services
{
    public interface IAccountService
    {
        //Task<bool> Login(dtoLogin login);
        Task<List<Userdto>> Read();
        Task<bool> Login(string user, string pass);
        Task Logout();
    }
}
