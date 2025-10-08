using SLibrary.DataAccess.Models;
using SLibrary.DataAccess.Interfacses;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SLibrary.DataAccess.Repositories
{
    public class DBUserRepository : IUserRepository
    {
        private readonly SLibararyDBContext _db;

        public DBUserRepository(SLibararyDBContext db)
        {
            _db = db;
        }

        public List<User> GetUsers()
        {
            List<User> users = _db.Users.ToList();
            return users;
        }

        public void Add(User user)
        {
            var exist = _db.Users.FirstOrDefault(x => x.Id == user.Id);
            if(exist != null)
                throw new Exception("Username already exists.");
            else
            {
                _db.Users.Add(user);
            }
            _db.SaveChanges();
        }

        public User GetByUsername(string name)
        {
            var user = _db.Users.FirstOrDefault(x => x.Username == name);
            return user;
        }
    }
}
