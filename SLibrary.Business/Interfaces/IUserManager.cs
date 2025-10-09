using Shared;
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
    }
}
