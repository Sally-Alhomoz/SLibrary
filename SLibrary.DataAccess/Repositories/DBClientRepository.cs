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
using Microsoft.EntityFrameworkCore;
namespace SLibrary.DataAccess.Repositories
{
    public class DBClientRepository : IClientRepository
    {
        private readonly SLibararyDBContext _db;
        private readonly ILogger<DBClientRepository> _logger;

        public DBClientRepository(SLibararyDBContext db, ILogger<DBClientRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public void Add(Client client)
        {
            var exist = _db.Clients.FirstOrDefault(x => x.Id == client.Id);
            if (exist != null)
            {
                _logger.LogWarning("User with Username already exist.", client.fullName);
                throw new Exception("Username already exists.");
            }
            else
            {
                _db.Clients.Add(client);
                _logger.LogInformation("Client added successfully.");
            }
        }
        public List<Client> GetClients()
        {
            List<Client> clients = _db.Clients.ToList();
            return clients;
        }

        public Client GetByPhoneNo(string no)
        {
            var client = _db.Clients.FirstOrDefault(c => c.PhoneNo == no);
            return client;
        }

        public Client GetByName(string name)
        {
            var client = _db.Clients.FirstOrDefault(c => c.fullName == name);
            return client;
        }


    }
}
