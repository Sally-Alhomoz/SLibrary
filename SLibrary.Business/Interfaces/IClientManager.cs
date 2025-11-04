using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLibrary.Business.Interfaces
{
    public interface IClientManager
    {
        void Add(Clientdto dto);
        Clientdto GetByPhoneNo(string no);
        List<Clientdto> GetAllClients();
        Clientdto GetByName(string name);
        public IEnumerable<Clientdto> SearchClients(string term);
    }
}
