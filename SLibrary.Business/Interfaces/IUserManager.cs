using Shared;
using SLibrary.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLibrary.Business.Interfaces
{
    public interface IUserManager
    {
        void Add(dtoNewUser u);
        bool Validatelogin(dtoLogin user);
        Userdto GetByUsername(string name);

        string Delete(string username);
        List<Userdto> GetAllUsers();
        bool VerifyPassword(string pass, string id, string storedhash);
        public bool SetUserInActive(string username);
        public bool ResetPassword(string username, string newpassword, string oldpassword);
        public bool ValidatePassword(string username, string password);
        public bool EditEmail(string username, string newemail);
        public Userdto GetAccountInfo(string username);
        bool toggleUserStatus(string username);
        (List<Userdto> users, int totalCount) GetUsersPaged(int page,
            int pageSize,
            string search,
            string sortBy,
            string sortDirection);
    }
}