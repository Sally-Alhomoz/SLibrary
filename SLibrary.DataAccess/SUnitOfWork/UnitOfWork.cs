using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Repositories;
using System;

namespace SLibrary.DataAccess.SUnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SLibararyDBContext _db;
        private readonly ILoggerFactory _logger;

        public IBookRepository DBBooks{ get; private set; }
        public IReservationRepository DBReservations { get; private set; }
        public IUserRepository DBUsers { get; private set; }
        public UnitOfWork(SLibararyDBContext db , ILoggerFactory logger)
        {
            _db = db;
            _logger = logger;

            var bookLogger = _logger.CreateLogger<DBBookRepository>();
            var resLogger = _logger.CreateLogger<DBReservationRepository>();
            var userLogger = _logger.CreateLogger<DBUserRepository>();

            DBBooks = new DBBookRepository(_db,bookLogger);
            DBReservations = new DBReservationRepository(_db, resLogger);
            DBUsers = new DBUserRepository(_db, userLogger);

        }

        public int Complete()
        {
            return _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
