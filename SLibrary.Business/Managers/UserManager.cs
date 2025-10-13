using Shared;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;
using SLibrary.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SLibrary.DataAccess.Repositories;


namespace SLibrary.Business.Managers
{
    public class UserManager : IUserManager
    {
        IUserRepository userRepo;
        private readonly ILogger<UserManager> _logger;

        public UserManager(IUserRepository repo, ILogger<UserManager> logger)
        {
            userRepo = repo;
            _logger = logger;
        }

        public void Add(dtoNewUser u)
        {
            _logger.LogInformation("Adding user");
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = u.Username,
                Email = u.Email,
                Password = u.Password,
                Role = Role.User,
                Checksum = ""
            };
            userRepo.Add(user);
            _logger.LogInformation("User added to the repository");
        }

        public bool Validatelogin(dtoLogin user)
        {
            _logger.LogInformation("Validating login for username: {Username}", user.Username);

            var exist = userRepo.GetByUsername(user.Username);
            if (exist == null)
            {
                _logger.LogWarning("Login failed. Username not found: {Username}", user.Username);
                return false;
            }

            bool flag = VerifyPassword(user.password, exist.Id.ToString(), exist.Password);
            if(flag)
            {
                _logger.LogInformation("Login successful for username: {Username}", user.Username);
            }
            else 
            {
                _logger.LogWarning("Login failed for username: {Username}", user.Username);
            }
            return flag;
        
           
        }

        public Userdto GetByUsername(string name)
        {
           _logger.LogInformation("Retriving a user by username: {Username}.", name);
            var exist = userRepo.GetByUsername(name);

            if(exist != null)
            {
                var user = new Userdto
                {
                    Id = exist.Id,
                    Username = exist.Username,
                    Role = exist.Role
                };
                _logger.LogInformation("Found user with username: {Username}.", name);
                return user;
            }
            _logger.LogWarning("User not found");
            return null;
        }

        public string Delete(string username)
        {
            _logger.LogInformation("Deleting a user.");

            var flag = userRepo.Delete(username);
            if (flag)
            {
                _logger.LogInformation("Deleting a user with username : {Username} successfully.", username);
                return "User deleted successfully";
            }

            _logger.LogInformation("Deleting a user with username : {Username} Failed.", username);
            return "Failed to delete user";
        }

        public List<Userdto> GetAllUsers()
        {
            _logger.LogInformation("Retrieving users from the database.");
            return userRepo.GetUsers().Select(x => new Userdto
            {
               Id = x.Id,
               Username=x.Username,
               Role=x.Role
            }).ToList();
        }

  

        public bool VerifyPassword(string pass, string id ,string storedhash)
        {
            _logger.LogInformation("Verifying password for user ID {UserId}.", id);
            var hashed = userRepo.VerifyPassword(pass, id, storedhash);

            if (hashed)
            {
                _logger.LogInformation("Password Verified successfully");
                return true;
            }
            _logger.LogWarning("Verifying password Failed");
            return false;
        }


    }
}
