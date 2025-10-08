using Shared;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;
using SLibrary.Business.Interfaces;
using System;

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
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = u.Username,
                Email = u.Email,
                Password = u.Password,
                Role = Role.User,
                Checksum = ""
            };
            userRepo.Add(user);
        }

        public bool Validatelogin(dtoLogin user)
        {
            var exist = userRepo.GetByUsername(user.Username);
            if (exist != null && exist.Password == user.password)
                return true;
            else
                return false;
           
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
    }
}
