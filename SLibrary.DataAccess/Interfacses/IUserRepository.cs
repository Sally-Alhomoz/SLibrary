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
        bool VerifyPassword(string pass, string id, string storedhash);
        void UpdateStatus(User user);
        public void RestPassword(User user);

        public bool ValidatePassword(User user, string password);

        public void EditEmail(User user);
        public bool EmailExist(string email);
        public void EditUsername(User user);
    }
}
