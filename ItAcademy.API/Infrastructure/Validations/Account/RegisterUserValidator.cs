using FluentValidation;
using ItAcademy.Application.Accounts.Requests;

namespace ItAcademy.API.Infrastructure.Validations.Account
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserValidator()
        {
            RuleFor(i => i.UserName).NotEmpty().Length(3, 50)
               .WithMessage("User Name must be between 3 and 50");
            RuleFor(i => i.Email).NotEmpty().Length(3, 50)
                .WithMessage("Email must be between 3 and 50")
                .EmailAddress().WithMessage("Right Email format is required!");
            RuleFor(i => i.PasswordHash).NotEmpty().Length(5, 100).WithMessage("Password Length Must be more than 5");
        }
    }
}
