using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SLibrary.DataAccess.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shared;
using SLibrary.Business.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using SLibrary.Business.Managers;
using static System.Reflection.Metadata.BlobBuilder;

namespace SLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientManager _clientManager;

        public ClientsController(IClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        ///<summary>
        ///Get all clients.
        ///</summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Read()
        {

            var clients = _clientManager.GetAllClients();
            return Ok(clients);
        }

        /// <summary>
        /// Get a client by phone number.
        /// </summary>
        [Authorize]
        [HttpGet("GetByPhoneNo/{phone}")]
        public async Task<IActionResult> GetByPhoneNo(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return BadRequest("Phone number is required.");

            var client = _clientManager.GetByPhoneNo(phone); 
            if (client == null)
                return NotFound($"No client found with phone number {phone}.");

            return Ok(client);
        }

        /// <summary>
        /// Get a client by full name.
        /// </summary>
        [Authorize]
        [HttpGet("{fullName}")] 
        public async Task<IActionResult> GetByName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return BadRequest("Client full name is required.");

            var client = _clientManager.GetByName(fullName);

            if (client == null)
                return NotFound($"No client found with name {fullName}.");

            return Ok(client);
        }

        /// <summary>
        /// Search clients by name or phone number for autocomplete purposes.
        /// </summary>
        [HttpGet("Search")]
        // Remove [Authorize] if this endpoint is being called by an AJAX script
        // from an authenticated page, to simplify client-side authentication handling.
        public IActionResult SearchClients([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Ok(new object[] { });
            }
            var clients = _clientManager.SearchClients(term);

            if (clients == null || !clients.Any())
            {
                return Ok(new object[] { });
            }

            var results = clients.Select(c => new
            {
                label = $"{c.fullName} ({c.PhoneNo})",
                value = c.PhoneNo,                     
                name = c.fullName,                    
                address = c.Address                    
            }).ToList();

            return Ok(results);
        }
    }
}
