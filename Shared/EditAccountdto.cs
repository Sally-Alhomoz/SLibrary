

namespace Shared
{
    public class EditAccountdto
    {
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
