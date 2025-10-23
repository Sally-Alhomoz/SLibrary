using System;

namespace Shared
{
    public class EditAccountdto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public bool IsActive { get; set; }

        public class EditEmail
        {
            public string newEmail { get; set; }
        }

        public class ResetPassword
        {
            public string OldPassword { get; set; }

            public string NewPassword { get; set; }

            public string ConfirmPassword { get; set; }
        }

        public EditEmail EditEmaildto { get; set; } = new EditEmail();
        public ResetPassword ResetPassworddto { get; set; } = new ResetPassword();
    }
}
