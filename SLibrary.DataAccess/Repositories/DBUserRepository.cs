using SLibrary.DataAccess.Models;
using SLibrary.DataAccess.Interfacses;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Shared;
using System.Text;
using Microsoft.Extensions.Logging;
using Azure.Core;

namespace SLibrary.DataAccess.Repositories
{
    public class DBUserRepository : IUserRepository
    {
        private readonly SLibararyDBContext _db;
        private readonly ILogger<DBUserRepository> _logger;

        public DBUserRepository(SLibararyDBContext db, ILogger<DBUserRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public List<User> GetUsers()
        {
            _logger.LogInformation("Retrieving users from the database.");
          
             List<User> users = _db.Users.ToList();
             _logger.LogInformation("Successfully retrieved {UserCount} users.", users.Count);
             return users;
  
        }

        public void Add(User user)
        {
            _logger.LogInformation("Adding user to the Database");

            var exist = _db.Users.FirstOrDefault(x => x.Id == user.Id);
            if (exist != null)
            {
                _logger.LogWarning("User with Username already exist.", user.Username);
                throw new Exception("Username already exists.");
            }
            else
            {
                user.Password=HashPassword(user.Password, user.Id.ToString());
                _db.Users.Add(user);
                _logger.LogInformation("User with ID {UserId} added successfully.", user.Id);
            }
            
        }

        public User GetByUsername(string name)
        {
            _logger.LogInformation("Retriving a user by username: {Username}.", name);

            var user = _db.Users.FirstOrDefault(x => x.Username == name);
            if(user != null)
            {
                _logger.LogInformation("User with username : {Username} found.",name);
            }
            else
            {
                _logger.LogWarning("No user Found");
            }
            return user;
        }


        public bool Delete(string username)
        {
            _logger.LogInformation("Deleting a user.");


             var user = _db.Users.FirstOrDefault(x => x.Username == username);

             if (user == null)
             {
                 _logger.LogWarning("No user Found with username : {Username}.",username);
                 return false;
             }

             _db.Users.Remove(user);
             _logger.LogInformation("User with username : {Username} deleted successfully.", user.Username);
             return true;
        }

        private string HashPassword(string pass, string id)
        {
            _logger.LogDebug("Hashing password for user ID {UserId}.", id);
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
            _logger.LogDebug("Verifying password for user ID {UserId}.", id);

            byte[] userid = Encoding.UTF8.GetBytes(id);

            var hashed = HashPassword(pass, id);

            bool result = (hashed == storedhash);


            _logger.LogDebug("Verifying password for user ID {UserId} : {result}", id,result);
            return result;
        }
    }
}
