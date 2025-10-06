using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class dtoLogin
    {
        [Required]
        public string userName { get; set; }
        [Required]
        public string password { get; set; }

    }
}
