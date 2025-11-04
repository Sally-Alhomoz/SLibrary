using Shared;

namespace SLibraryBlazor.Services
{
    public interface IReservationService
    {
        Task<List<Reservationdto>> Read();
    }
}
