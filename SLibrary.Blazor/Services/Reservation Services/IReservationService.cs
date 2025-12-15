using Shared;

namespace SLibrary.Blazor.Services.Reservation_Services
{
    public interface IReservationService
    {
        Task<(List<Reservationdto> Items, int TotalCount)> Read(int page, int pageSize, string search, string sortBy, string sortDirection);
        Task<bool> Release(string clientname,string title);
        Task<Clientdto> GetClientInfo(string username);
    }
}
