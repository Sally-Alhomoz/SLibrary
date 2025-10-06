using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class dtoNewUser
    {
        [Required]
        public string userName { get; set; }
        [Required]
        public string password { get; set; }

        [Required]
        public string Email { get; set; }

    }
}
