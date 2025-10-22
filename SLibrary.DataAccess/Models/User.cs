using System.ComponentModel.DataAnnotations;
using Shared;
using System;

namespace SLibrary.DataAccess.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }

        public Role Role { get; set; }
        public string Checksum { get; set; }

        public bool IsActive { get; set; }
    }

}
