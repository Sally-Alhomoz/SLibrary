using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;

namespace SLibrary.DataAccess.Repositories
{
    public class DBReservationRepository : IReservationRepository
    {
        private readonly SLibararyDBContext _db;
        private readonly ILogger<DBReservationRepository> _logger;


        public DBReservationRepository(SLibararyDBContext db, ILogger<DBReservationRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public List<Reservation> Load()
        {
            List<Reservation> reservations = _db.Reservations.ToList();
            return reservations;
        }

        public void Add(Reservation r)
        {
            _logger.LogInformation("Adding a reservation");
            _db.Reservations.Add(r);
            _logger.LogInformation("Reservation added successfully");
        }

        public List<Reservation> GetReservations()
        {
            _logger.LogInformation("Retrieving reservation");
            List<Reservation> reservations = _db.Reservations.ToList();
            return reservations;
        }

        public Reservation GetActiveReservation(string title, string clientName)
        {
            _logger.LogInformation("Retrieving active reservations for book with title {title}",title);
            var res = _db.Reservations.FirstOrDefault(x => x.BookTitle == title && x.ClientName == clientName && x.ReleaseDate == null);

            if(res != null)
            {
                _logger.LogInformation("Active reservations for book with title {title} retrieved successfully", title);
                return res;
            }
            _logger.LogWarning("No active reservations for book with title {title}", title);
            return null;
        }

        public void Update(Reservation r)
        {
            _logger.LogInformation("Updating a reservation");
            _db.Reservations.Update(r);
            _logger.LogInformation("Reservation updated successfully");
        }

        public Reservation GetReservationById(int id)
        {
            _logger.LogInformation("get reservation by id : {id}", id);
            var res = _db.Reservations.Find(id);
            if(res ==null)
            {
                _logger.LogWarning("Reservation not found");
                return null;
            }
            _logger.LogInformation("Reservation with id : {id} found", id);
            return res;
        }

    }
}
