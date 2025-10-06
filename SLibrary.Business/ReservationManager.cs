using SLibrary.DataAccess;
using SLibrary.DataAccess.Interfacses;
using System;
using System.Collections.Generic;
using SLibrary.DataAccess.Models;
using Shared;
using System.Linq;
using System.Collections.Immutable;
using SLibrary.Business.Interfaces;

namespace SLibrary.Business
{
    public class ReservationManager : IReservationManager
    {
        IReservationRepository reservationRepo;
        IBookRepository bookRepo;

        public ReservationManager(IReservationRepository Rrepo , IBookRepository Brepo )
        {
            reservationRepo = Rrepo;
            bookRepo = Brepo;
        }

        public string ReserveBook(string title, string clientName)
        {
            Book temp = bookRepo.GetByName(title);
            if (temp != null)
            {
                if (temp.Available > 0)
                {
                    bookRepo.UpdateCounts(temp.ID, temp.Available - 1, temp.Reserved + 1);

                    Reservation r = new Reservation
                    {
                        ClientName = clientName,
                        BookTitle = title,
                        BookID = temp.ID,
                        ReservedDate = DateTime.Now,
                        ReleaseDate = null
                    };
                    reservationRepo.Add(r);
                    return "Book Reserved Successfully !!\n";
                }
                else
                    return "Book Can Not be Reserved\n";
            }
            return "Book Not Found\n";
        }

        public string ReleaseBook(string title, string clientName)
        {
            Book temp = bookRepo.GetByName(title);
            if (temp != null)
            {
                if (temp.Reserved > 0)
                {
                    Reservation res = reservationRepo.GetActiveReservation(title, clientName);

                    if (res == null)
                        return "No active reservation found !!\n";

                    bookRepo.UpdateCounts(temp.ID, temp.Available + 1, temp.Reserved - 1);

                    res.ReleaseDate = DateTime.Now;
                    reservationRepo.Update(res);
                    return "Book Released Successfully !!\n";
                }
                else
                    return "There is NO Book to Release\n";
            }
            return "Book Not Found\n";
        }
        public List<Reservationdto> GetAllReservations()
        {
            return reservationRepo.GetReservations().Select(r => new Reservationdto
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
            var res = reservationRepo.GetReservationById(id);
            var resdto = new Reservationdto
            {
                ID = res.ID,
                ClientName= res.ClientName,
                BookID= res.BookID,
                BookTitle=res.BookTitle,
                ReservedDate= res.ReservedDate,
                ReleaseDate= res.ReleaseDate
            };
            return resdto;
        }
    }
}
