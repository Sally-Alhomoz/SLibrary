using Shared;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;
using SLibrary.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;


namespace SLibrary.Business.Managers
{
    public class UserManager : IUserManager
    {
        IUserRepository userRepo;

        public UserManager(IUserRepository repo)
        {
            userRepo = repo;
        }

        public void Add(dtoNewUser u)
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = u.Username,
                Email = u.Email,
                Password = HashPassword(u.Password, userId.ToString()),
                Role = Role.User,
                Checksum = ""
            };
            userRepo.Add(user);
        }

        public bool Validatelogin(dtoLogin user)
        {
            var exist = userRepo.GetByUsername(user.Username);
            if (exist == null)
                return false;

            bool flag = VerifyPassword(user.password, exist.Id.ToString(), exist.Password);
            return flag;
        
           
        }

        public Userdto GetByUsername(string name)
        {
            var exist = userRepo.GetByUsername(name);

            if(exist != null)
            {
                var user = new Userdto
                {
                    Id = exist.Id,
                    Username = exist.Username,
                    Role = exist.Role
                };
                return user;
            }
            return null;
        }

        public string Delete(string username)
        {
            var flag = userRepo.Delete(username);
            if (flag)
                return "User deleted successfully";

            return "Failed to delete user";
        }

        public List<Userdto> GetAllUsers()
        {
            return userRepo.GetUsers().Select(x => new Userdto
            {
               Id = x.Id,
               Username=x.Username,
               Role=x.Role
            }).ToList();
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

        public bool VerifyPassword(string pass, string id ,string storedhash)
        {
            byte[] userid = Encoding.UTF8.GetBytes(id);

            var hashed = HashPassword(pass, id);

            if (hashed == storedhash)
                return true;
            return false;
        }


    }
}
