using SLibrary.DataAccess.Models;
using System.Collections.Generic;

namespace SLibrary.DataAccess.Interfacses
{
    public interface IUserRepository
    {
        List<User> GetUsers();
        void Add(User user);
        User GetByUsername(string name);

        bool Delete(string username);
    }
}
