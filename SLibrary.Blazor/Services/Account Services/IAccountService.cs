using Shared;

namespace SLibrary.Blazor.Services.Account_Services
{
    public interface IAccountService
    {
        Task<(List<Userdto> Items, int TotalCount)> Read(int page, int pageSize , string search , string sortBy, string sortDirection );
        Task<bool> Login(string user, string pass);
        Task Logout();
        Task Delete(string username);
        Task<Userdto> GetAccountInfo();
        Task EditEmail(string NewEmail);
        Task ChangePassword(ResetPassworddto dto);
        Task Register(dtoNewUser dto);
    }
}
