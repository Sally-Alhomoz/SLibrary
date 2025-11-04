using Shared;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Models;
using SLibrary.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SLibrary.DataAccess.SUnitOfWork;
using System.Globalization;

namespace SLibrary.Business.Managers
{
    public class ClientManager : IClientManager
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ClientManager> _logger;

        public ClientManager(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public List<Clientdto> GetAllClients()
        {
            return _uow.DBClients.GetClients().Select(x => new Clientdto
            {
                Id = x.Id,
                fullName=x.fullName,
                PhoneNo= x.PhoneNo,
                Address = x.Address
            }).ToList();
        }

        public void Add(Clientdto dto)
        {
            var client = new Client
            {
                fullName=dto.fullName,
                PhoneNo=dto.PhoneNo,
                Address=dto.Address
            };
            _uow.DBClients.Add(client);
            _uow.Complete();
        }

        public Clientdto GetByPhoneNo(string no)
        {
            var exist = _uow.DBClients.GetByPhoneNo(no);

            if (exist != null)
            {
                var clinet = new Clientdto
                {
                    Id = exist.Id,
                    fullName=exist.fullName,
                    PhoneNo=exist.PhoneNo,
                    Address=exist.Address
                };
                return clinet;
            }
            return null;
        }

        public Clientdto GetByName(string name)
        {
            var exist = _uow.DBClients.GetByName(name);

            if (exist != null)
            {
                var clinet = new Clientdto
                {
                    Id = exist.Id,
                    fullName = exist.fullName,
                    PhoneNo = exist.PhoneNo,
                    Address = exist.Address
                };
                return clinet;
            }
            return null;
        }


        public IEnumerable<Clientdto> SearchClients(string term)
        {
            string searchTerm = term?.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<Clientdto>();
            }

            var clients = _uow.DBClients.GetClients()
                .Where(c => c.fullName.ToLower().Contains(searchTerm) ||
                            c.PhoneNo.Contains(searchTerm))
                .ToList();

            var clientDtos = clients.Select(c => new Clientdto
            {            
                fullName = c.fullName,
                PhoneNo = c.PhoneNo,
                Address = c.Address
            }).ToList(); 

            return clientDtos;
        }
    }
}
