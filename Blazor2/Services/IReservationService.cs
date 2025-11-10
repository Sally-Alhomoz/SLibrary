using Shared;

namespace Blazor2.Services
{
    public interface IReservationService
    {
        Task<List<Reservationdto>> Read();
        Task<(bool, string)> Release(string clientname, string title);
    }
}
