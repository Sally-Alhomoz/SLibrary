using Shared;

namespace SLibraryBlazor.Services
{
    public interface IReservationService
    {
        Task<List<Reservationdto>> Read();
         Task<(bool, string)> Release(string clientname, string title);
    }
}
