using SLibrary.DataAccess.Models;
using SLibrary.DataAccess.Interfacses;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Shared;
using System.Text;

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


        public bool Delete(string username)
        {
            var user = _db.Users.FirstOrDefault(x => x.Username == username);

            if (user == null)
                return false;

            _db.Users.Remove(user);
            _db.SaveChanges();
            return true;
        }

        private string HashPassword(string pass, string id)
        {
            byte[] userid = Encoding.UTF8.GetBytes(id);

            byte[] hashed = KeyDerivation.Pbkdf2(
                password: pass,
                salt: userid,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32);

            return Convert.ToBase64String(hashed);
        }

        public bool VerifyPassword(string pass, string id, string storedhash)
        {
            byte[] userid = Encoding.UTF8.GetBytes(id);

            var hashed = HashPassword(pass, id);

            if (hashed == storedhash)
                return true;
            return false;
        }
    }
}
