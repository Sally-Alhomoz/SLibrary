using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class dtoNewUser
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

    }
}
