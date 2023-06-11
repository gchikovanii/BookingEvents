using ItAcademy.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;

namespace ItAcademy.Application.Accounts.Helper
{
    public class PasswordHasher : IPasswordHasher<AppUser>
    {
        public string HashPassword(AppUser user, string password)
        {
            return password;
        }
        public PasswordVerificationResult VerifyHashedPassword(AppUser user, string hashedPassword, string providedPassword)
        {
            return hashedPassword.Equals(providedPassword) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
    }
}
