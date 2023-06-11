using System.Security.Cryptography;
using System.Text;
using ItAcademy.Application.Infrastructure.Errors.CustomExceptions;
using ItAcademy.Application.Infrastructure.Localization.Errors;

namespace ItAcademy.Application.Accounts.Helper
{
    public static class PasswordHashGenerator
    {
        private const string Key = "Alxamdulila";
        //Hash Password using sha 256
        public static string HashPassword(string password)
        {
            using (var s = SHA256.Create())
            {
                var bytes = Encoding.ASCII.GetBytes(password + Key);
                var hashBytes = s.ComputeHash(bytes);
                var stringBuilder = new StringBuilder();
                for (var i = 0; i < hashBytes.Length; i++)
                {
                    stringBuilder.Append(hashBytes[i].ToString("X2"));
                }
                if (string.IsNullOrEmpty(stringBuilder.ToString()))
                    throw new ResultWasEmptyException(ErrorMessages.EmptyString);
                return stringBuilder.ToString();
                }
            }
    }
}
