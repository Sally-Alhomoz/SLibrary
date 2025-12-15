using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using SLibrary.Business.Interfaces;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;
using SLibrary.DataAccess.SUnitOfWork;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;


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
                IsActive = false
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
            if (flag)
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

            if (exist != null)
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
                Username = x.Username,
                Role = x.Role,
                IsActive = x.IsActive
            }).ToList();
        }



        public bool VerifyPassword(string pass, string id, string storedhash)
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

        public bool ResetPassword(string username, string newpassword, string oldpassword)
        {
            _logger.LogInformation("Reset the password for user {Username}", username);

            var user = _uow.DBUsers.GetByUsername(username);

            if (user == null)
            {
                _logger.LogWarning("User not found: {Username}", username);
                return false;
            }

            var result = ValidatePassword(username, oldpassword);

            if (!result)
            {
                _logger.LogWarning("Incorrect old password for {Username}", username);
                return false;
            }

            user.Password = newpassword;
            _uow.DBUsers.RestPassword(user);
            _uow.Complete();

            _logger.LogInformation("Successfully reset the password for user {Username}", username);
            return true;
        }

        public bool ValidatePassword(string username, string password)
        {
            var user = _uow.DBUsers.GetByUsername(username);

            if (user == null)
            {
                return false;
            }

            return _uow.DBUsers.ValidatePassword(user, password);
        }


        public bool EditEmail(string username, string newemail)
        {
            var result = _uow.DBUsers.EmailExist(newemail);

            if (result)
            {
                _logger.LogWarning("Email already exist");
                return false;
            }

            var user = _uow.DBUsers.GetByUsername(username);
            if (user == null)
            {
                _logger.LogWarning("User not found: {Username}", username);
                return false;
            }
            user.Email = newemail;
            _uow.DBUsers.EditEmail(user);
            _uow.Complete();
            _logger.LogInformation("Successfully edit the email for user {Username}", username);
            return true;
        }

        public Userdto GetAccountInfo(string username)
        {
            var user = _uow.DBUsers.GetByUsername(username);

            if (user != null)
            {
                Userdto dto = new Userdto
                {
                    Id = user.Id,
                    IsActive = user.IsActive,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                };
                return dto;
            }
            return null;
        }

        public bool toggleUserStatus(string username)
        {
            var user = _uow.DBUsers.GetByUsername(username);
            if (user == null)
            {
                return false;
            }

            user.IsActive = !user.IsActive;
            _uow.Complete();
            return true;
        }

        public (List<Userdto> users, int totalCount) GetUsersPaged(
            int page = 1,
            int pageSize = 10,
            string search = "",
            string sortBy = "username",
            string sortDirection = "asc")
        {
            var query = _uow.DBUsers.GetUsers().AsQueryable();

            // GLOBAL SEARCH — all fields
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(u =>
                    u.Username.ToLower().Contains(s) ||
                    (u.Email != null && u.Email.ToLower().Contains(s)) ||
                    u.Role.ToString().ToLower().Contains(s) ||
                    (u.IsActive ? "active" : "inactive").Contains(s)
                );
            }

            // FULL SORTING — every column
            query = (sortBy?.ToLower(), sortDirection?.ToLower()) switch
            {
                ("email", "desc") => query.OrderByDescending(u => u.Email ?? ""),
                ("email", _) => query.OrderBy(u => u.Email ?? ""),
                ("role", "desc") => query.OrderByDescending(u => u.Role),
                ("role", _) => query.OrderBy(u => u.Role),
                ("isactive", "desc") => query.OrderByDescending(u => u.IsActive),
                ("isactive", _) => query.OrderBy(u => u.IsActive),
                (_, "desc") => query.OrderByDescending(u => u.Username),
                _ => query.OrderBy(u => u.Username)
            };

            int totalCount = query.Count();

            var users = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new Userdto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive
                })
                .ToList();

            return (users, totalCount);
        }

        // Helper for sorting
        private static Expression<Func<User, object>> GetSortExpression(string sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "email" => u => u.Email ?? "",
                "role" => u => u.Role,
                "isactive" => u => u.IsActive,
                _ => u => u.Username
            };
        }
    }
}