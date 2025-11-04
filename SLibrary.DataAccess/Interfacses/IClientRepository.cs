using SLibrary.DataAccess.Models;
using System.Collections.Generic;

namespace SLibrary.DataAccess.Interfacses
{
    public interface IClientRepository
    {
        List<Client> GetClients();
        Client GetByPhoneNo(string no);
        void Add(Client client);
        Client GetByName(string name);
    }
}
