using Shared;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;
using SLibrary.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SLibrary.DataAccess.SUnitOfWork;


namespace SLibrary.Business.Managers
{
    public class UserManager : IUserManager
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UserManager> _logger;

        public UserManager(IUnitOfWork uow, ILogger<UserManager> logger)
        {
            _uow = uow;
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
                Checksum = "",
                IsActive=false
            };
            _uow.DBUsers.Add(user);
            _logger.LogInformation("User added to the repository");
            _uow.Complete();
        }

        public bool Validatelogin(dtoLogin user)
        {
            _logger.LogInformation("Validating login for username: {Username}", user.Username);

            var exist = _uow.DBUsers.GetByUsername(user.Username);
            if (exist == null)
            {
                _logger.LogWarning("Login failed. Username not found: {Username}", user.Username);
                return false;
            }

            bool flag = VerifyPassword(user.password, exist.Id.ToString(), exist.Password);
            if(flag)
            {
                exist.IsActive = true;
                _uow.DBUsers.UpdateStatus(exist);
                _logger.LogInformation("Login successful for username: {Username}", user.Username);
            }
            else 
            {
                _logger.LogWarning("Login failed for username: {Username}", user.Username);
            }
            _uow.Complete();
            return flag;
           
        }

        public Userdto GetByUsername(string name)
        {
           _logger.LogInformation("Retriving a user by username: {Username}.", name);
            var exist = _uow.DBUsers.GetByUsername(name);

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

            var flag = _uow.DBUsers.Delete(username);

            if (flag)
            {
                _uow.Complete();
                _logger.LogInformation("Deleting a user with username : {Username} successfully.", username);
                return "User deleted successfully";
            }

            _logger.LogInformation("Deleting a user with username : {Username} Failed.", username);
            return "Failed to delete user";
        }

        public List<Userdto> GetAllUsers()
        {
            _logger.LogInformation("Retrieving users from the database.");
            return _uow.DBUsers.GetUsers().Select(x => new Userdto
            {
               Id = x.Id,
               Username=x.Username,
               Role=x.Role,
               IsActive=x.IsActive
            }).ToList();
        }

  

        public bool VerifyPassword(string pass, string id ,string storedhash)
        {
            _logger.LogInformation("Verifying password for user ID {UserId}.", id);
            var hashed = _uow.DBUsers.VerifyPassword(pass, id, storedhash);

            if (hashed)
            {
                _logger.LogInformation("Password Verified successfully");
                return true;
            }
            _logger.LogWarning("Verifying password Failed");
            return false;
        }

        public bool SetUserInActive(string username)
        {
            _logger.LogInformation("Setting active status for user {Username} to false", username);

            var user = _uow.DBUsers.GetByUsername(username);

            if (user == null)
            {
                _logger.LogWarning("User not found: {Username}", username);
                return false;
            }

            user.IsActive = false;
            _uow.DBUsers.UpdateStatus(user);
            _uow.Complete();

            _logger.LogInformation("User status updated successfully for {Username}", username);
            return true;

        }

    }
}
