using System;

namespace Shared
{
    public class EditAccountdto
    {
        //to display user info
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class EditEmaildto
    {
        public string newEmail { get; set; }
    }
}
