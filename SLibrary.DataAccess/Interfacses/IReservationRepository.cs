using System.Collections.Generic;
using SLibrary.DataAccess.Models;

namespace SLibrary.DataAccess.Interfacses
{
    public interface IReservationRepository
    {
        List<Reservation> Load();
        void Add(Reservation r);
        List<Reservation> GetReservations();
        Reservation GetActiveReservation(string title, string clientName);
        void Update(Reservation r);
        Reservation GetReservationById(int id);
    }
}
