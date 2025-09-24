using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace SLibrary.DataAccess
{
    public interface IReservationRepository
    {
        List<Reservation> Load();
        void Add(Reservation r);
        List<Reservation> GetReservation();
        Reservation GetActiveReservation(string title, string clientName);
        void Update(Reservation r);
    }
}
