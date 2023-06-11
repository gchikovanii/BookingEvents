using System.ComponentModel.DataAnnotations;

namespace ItAcademy.Application.Accounts.Requests
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
