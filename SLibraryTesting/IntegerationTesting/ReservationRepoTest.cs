using System;
using System.Linq;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SLibrary.DataAccess;
using SLibrary.DataAccess.Models;
using SLibrary.DataAccess.Repositories;
using Xunit;

namespace SLibraryTesting.IntegerationTesting
{
    public class ReservationRepoTest
    {
        private readonly SLibararyDBContext _db;
        private readonly DBReservationRepository _repo;
        private readonly ILogger<DBReservationRepository> _logger;
        public ReservationRepoTest()
        {
            var options = new DbContextOptionsBuilder<SLibararyDBContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _db = new SLibararyDBContext(options);

            _db.Reservations.Add(new Reservation
            {
                ID = 1,
                BookTitle = "Book1",
                ClientName = "Client1",
                ReservedDate = DateTime.Now,
                ReleaseDate = null
            });
            _db.Reservations.Add(new Reservation
            {
                ID = 2,
                BookTitle = "Book2",
                ClientName = "Client2",
                ReservedDate = DateTime.Now,
                ReleaseDate = DateTime.Now.AddHours(-1)
            });
            _db.SaveChanges();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<DBReservationRepository>();

            _repo = new DBReservationRepository(_db, _logger);
        }

        [Fact]
        public void Add_ShouldAddNewReservation()
        {
            var res = new Reservation
            {
                ID = 3,
                BookTitle = "Book3",
                ClientName = "Client3",
                ReservedDate = DateTime.Now
            };

            _repo.Add(res);
            _db.SaveChanges();

            var result = _db.Reservations.Find(3);
            Assert.NotNull(result);
            Assert.Equal("Book3", result.BookTitle);
        }

        [Fact]
        public void GetReservations_ShouldReturnAllReservations()
        {
            var result = _repo.GetReservations();
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetActiveReservation_ShouldReturnReservation_WhenExists()
        {
            var result = _repo.GetActiveReservation("Book1", "Client1");
            Assert.NotNull(result);
            Assert.Equal("Book1", result.BookTitle);
        }


        [Fact]
        public void GetActiveReservation_ShouldReturnNull_WhenNotexit()
        {
            var none = _repo.GetActiveReservation("Book2", "Client2");
            Assert.Null(none);
        }

        [Fact]
        public void Update()
        {
            var res = _db.Reservations.Find(1);
            res.ReleaseDate = DateTime.Now;

            _repo.Update(res);
            _db.SaveChanges();

            var updated = _db.Reservations.Find(1);
            Assert.NotNull(updated.ReleaseDate);
        }

        [Fact]
        public void GetReservationById_ShouldReturnReservation_WhenExist()
        {
            var res = _repo.GetReservationById(1);
            Assert.NotNull(res);
            Assert.Equal("Client1", res.ClientName);
        }

        [Fact]
        public void GetReservationById_ShouldReturnNull_WhenNotExist()
        {
            var none = _repo.GetReservationById(999);
            Assert.Null(none);
        }
    }
}
