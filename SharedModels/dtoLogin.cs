using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class dtoLogin
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string password { get; set; }

    }
}
