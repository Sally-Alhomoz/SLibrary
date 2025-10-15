using SLibrary.DataAccess;
using SLibrary.DataAccess.Interfacses;
using System;
using System.Collections.Generic;
using SLibrary.DataAccess.Models;
using Shared;
using System.Linq;
using System.Collections.Immutable;
using SLibrary.Business.Interfaces;
using Microsoft.Extensions.Logging;
using SLibrary.DataAccess.SUnitOfWork;

namespace SLibrary.Business.Managers
{
    public class ReservationManager : IReservationManager
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ReservationManager> _logger;

        public ReservationManager(IUnitOfWork uow ,ILogger<ReservationManager> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public string ReserveBook(string title, string clientName)
        {
            _logger.LogInformation("Reserving a book wit title : {title}", title);
            Book temp = _uow.DBBooks.GetByName(title);
            if (temp != null)
            {
                if (temp.isDeleted)
                {
                    _logger.LogWarning("Book with title : {title} Not found", title);
                    return "Book Not Found - Deleted\n";
                }
                else if (temp.Available > 0)
                {
                    _uow.DBBooks.UpdateCounts(temp.ID, temp.Available - 1, temp.Reserved + 1);

                    Reservation r = new Reservation
                    {
                        ClientName = clientName,
                        BookTitle = title,
                        BookID = temp.ID,
                        ReservedDate = DateTime.Now,
                        ReleaseDate = null
                    };
                    _uow.DBReservations.Add(r);
                    _uow.Complete();
                    _logger.LogInformation("Book with title : {title} Reserved Successfully", title);
                    return "Book Reserved Successfully !!\n";
                }
                else
                {
                    _logger.LogWarning("Book with title : {title} Can Not be Reserved", title);
                    return "Book Can Not be Reserved\n";
                }
            }
            _logger.LogWarning("Book with title : {title} Not found", title);
            return "Book Not Found\n";
        }

        public string ReleaseBook(string title, string clientName)
        {
            _logger.LogInformation("Releasing a book wit title : {title}", title);
            Book temp = _uow.DBBooks.GetByName(title);
            if (temp != null)
            {
                if (temp.Reserved > 0)
                {
                    Reservation res = _uow.DBReservations.GetActiveReservation(title, clientName);

                    if (res == null)
                    {
                        _logger.LogWarning("Book with title : {title} has not active reservations", title);
                        return "No active reservation found !!\n";
                    }

                    _uow.DBBooks.UpdateCounts(temp.ID, temp.Available + 1, temp.Reserved - 1);

                    res.ReleaseDate = DateTime.Now;
                    _uow.DBReservations.Update(res);
                    _uow.Complete();
                    _logger.LogInformation("Book with title : {title} Rleased Successfully", title);
                    return "Book Released Successfully !!\n";
                }
                else
                {
                    _logger.LogWarning("Book with title : {title} Can Not be Released", title);
                    return "There is NO Book to Release\n";
                }
            }
            _logger.LogWarning("Book with title : {title} Not found", title);
            return "Book Not Found\n";
        }
        public List<Reservationdto> GetAllReservations()
        {
            _logger.LogInformation("Retrieving reservations from the database.");
            return _uow.DBReservations.GetReservations().Select(r => new Reservationdto
            {
                ID = r.ID,
                ClientName = r.ClientName,
                BookID = r.BookID,
                BookTitle = r.BookTitle,
                ReservedDate = r.ReservedDate,
                ReleaseDate = r.ReleaseDate
            }).ToList();
        }

        public Reservationdto GetReservationById(int id)
        {
            _logger.LogInformation("Retrieving reservation with id {id}",id);
            var res = _uow.DBReservations.GetReservationById(id);
            var resdto = new Reservationdto
            {
                ID = res.ID,
                ClientName = res.ClientName,
                BookID = res.BookID,
                BookTitle = res.BookTitle,
                ReservedDate = res.ReservedDate,
                ReleaseDate = res.ReleaseDate
            };
            return resdto;
        }
    }
}
