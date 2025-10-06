using System;
using System.Collections.Generic;
using System.Linq;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;

namespace SLibrary.DataAccess.Repositories
{
    public class DBReservationRepository : IReservationRepository
    {
        private readonly SLibararyDBContext _db;

        public DBReservationRepository(SLibararyDBContext db)
        {
            _db = db;
        }

        public List<Reservation> Load()
        {
            List<Reservation> reservations = _db.Reservations.ToList();
            return reservations;
        }

        public void Add(Reservation r)
        {
            _db.Reservations.Add(r);
            _db.SaveChanges();
        }

        public List<Reservation> GetReservations()
        {
            List<Reservation> reservations = _db.Reservations.ToList();
            return reservations;
        }

        public Reservation GetActiveReservation(string title, string clientName)
        {
            var res = _db.Reservations.FirstOrDefault(x => x.BookTitle == title && x.ClientName == clientName && x.ReleaseDate == null);

            if(res != null)
            {
                return res;
            }
            return null;
        }

        public void Update(Reservation r)
        {
            _db.Reservations.Update(r);
            _db.SaveChanges();
        }

        public Reservation GetReservationById(int id)
        {
            var res = _db.Reservations.Find(id);
            return res;
        }

    }
}
