using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using SLibrary.Business.Interfaces;
using SLibrary.DataAccess;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;
using SLibrary.DataAccess.SUnitOfWork;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

        public string ReserveBook(string title, string clientName , string username , string phoneNo ,string address)
        {
            _logger.LogInformation("Reserving a book wit title : {title}", title);
            Book temp = _uow.DBBooks.GetByName(title);

            var exist = _uow.DBClients.GetByPhoneNo(phoneNo);

            if (temp != null)
            {
                if (temp.isDeleted)
                {
                    _logger.LogWarning("Book with title : {title} Not found", title);
                    return "Book Not Found - Deleted\n";
                }
                else if (temp.Available > 0)
                {
                    if(exist == null)
                    {
                        exist = new Client
                        {
                            fullName = clientName,
                            PhoneNo = phoneNo,
                            Address = address 
                        };
                        _uow.DBClients.Add(exist);
                        _uow.Complete();
                    }

                    _uow.DBBooks.UpdateCounts(temp.ID, temp.Available - 1, temp.Reserved + 1);

                    Reservation r = new Reservation
                    {
                        ReservedBy = username,
                        ClientName = exist.fullName,
                        ClientId= exist.Id,
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
                ReservedBy=r.ReservedBy,
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
                ReservedBy=res.ReservedBy,
                ClientName = res.ClientName,
                BookID = res.BookID,
                BookTitle = res.BookTitle,
                ReservedDate = res.ReservedDate,
                ReleaseDate = res.ReleaseDate
            };
            return resdto;
        }

        public (List<Reservationdto> items, int totalCount) GetReservationsPaged(
            int page = 1,
            int pageSize = 10,
            string search = "",
            string sortBy = "reservedDate",
            string sortDirection = "asc")
        {
            var query = _uow.DBReservations.GetReservations().AsQueryable();

            // Global search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(r =>
                    r.ReservedBy.ToLower().Contains(s) ||
                    r.ClientName.ToLower().Contains(s) ||
                    r.BookTitle.ToLower().Contains(s) 
                );
            }

            // Sorting
            query = (sortBy?.ToLower(), sortDirection?.ToLower()) switch
            {
                ("reservedby", "desc") => query.OrderByDescending(r => r.ReservedBy),
                ("reservedby", _) => query.OrderBy(r => r.ReservedBy),
                ("clientname", "desc") => query.OrderByDescending(r => r.ClientName),
                ("clientname", _) => query.OrderBy(r => r.ClientName),
                ("booktitle", "desc") => query.OrderByDescending(r => r.BookTitle),
                ("booktitle", _) => query.OrderBy(r => r.BookTitle),
                ("reserveddate", "desc") => query.OrderByDescending(r => r.ReservedDate),
                ("reserveddate", _) => query.OrderBy(r => r.ReservedDate),
                ("releasedate", "desc") => query.OrderByDescending(r => r.ReleaseDate ?? DateTime.MaxValue),
                ("releasedate", _) => query.OrderBy(r => r.ReleaseDate ?? DateTime.MaxValue),
                _ => query.OrderByDescending(r => r.ReservedDate)
            };

            int totalCount = query.Count();

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new Reservationdto
                {
                    ID = r.ID,
                    ReservedBy = r.ReservedBy,
                    ClientName = r.ClientName,
                    BookTitle = r.BookTitle,
                    ReservedDate = r.ReservedDate,
                    ReleaseDate = r.ReleaseDate,
                })
                .ToList();

            return (items, totalCount);
        }

        public int GetTotalReservationsByUser(string username)
        {
            _logger.LogInformation("Calculating total reservations for user: {Username}.", username);

            int totalCount = _uow.DBReservations.GetReservations()
                .Count(r => r.ReservedBy == username);

            _logger.LogInformation("User {Username} has a total of {Count} reservations.", username, totalCount);
            return totalCount;
        }

        public int GetActiveReservationsByUser(string username)
        {
            _logger.LogInformation("Calculating active reservations for user: {Username}.", username);

            int activeCount = _uow.DBReservations.GetReservations()
                .Count(r => r.ReservedBy == username && r.ReleaseDate == null);

            _logger.LogInformation("User {Username} has {Count} active reservations.", username, activeCount);
            return activeCount;
        }
    }
}
