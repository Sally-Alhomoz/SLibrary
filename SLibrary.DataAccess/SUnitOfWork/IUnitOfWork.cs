using System;
using SLibrary.DataAccess.Interfacses;

namespace SLibrary.DataAccess.SUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository DBBooks { get; }
        public IReservationRepository DBReservations { get;}
        public IUserRepository DBUsers { get; }
        public IClientRepository DBClients { get; }
        int Complete();
    }
}
