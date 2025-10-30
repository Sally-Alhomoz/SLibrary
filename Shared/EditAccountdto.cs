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

        //for edit email
        public string newEmail { get; set; }

        //for edit username
        public string newUsername { get; set; }
    }
}
