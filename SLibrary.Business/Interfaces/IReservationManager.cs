using Shared;
using System.Collections.Generic;


namespace SLibrary.Business.Interfaces
{
    public interface IReservationManager
    {
        string ReserveBook(string title, string clientName, string username);
        string ReleaseBook(string title, string clientName);
        List<Reservationdto> GetAllReservations();
        Reservationdto GetReservationById(int id);
    }
}
