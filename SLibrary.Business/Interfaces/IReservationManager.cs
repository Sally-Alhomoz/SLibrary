using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SLibrary.Business.Interfaces
{
    public interface IReservationManager
    {
        string ReserveBook(string title, string clientName, string username, string phoneNo, string address);
        string ReleaseBook(string title, string clientName);
        List<Reservationdto> GetAllReservations();
        Reservationdto GetReservationById(int id);
        (List<Reservationdto> items, int totalCount) GetReservationsPaged(
                    int page = 1,
                    int pageSize = 10,
                    string search = "",
                    string sortBy = "reservedDate",
                    string sortDirection = "asc");
    }
}
