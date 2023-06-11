namespace ItAcademy.Application.Accounts.Requests
{
    public class LogInUserRequest
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
