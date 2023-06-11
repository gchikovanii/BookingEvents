using FluentValidation;
using ItAcademy.Application.Events.Request;

namespace ItAcademy.API.Infrastructure.Validations.Events
{
    public class CreateEventValdiation : AbstractValidator<EventRequest>
    {
        public CreateEventValdiation()
        {
            RuleFor(i => i.Title).Length(1, 50).WithMessage("Title must be short!");
            RuleFor(i => i.Description).Length(10, 1000).WithMessage("Description must be between 10 and 1000!");
            RuleFor(i => i.Price).GreaterThanOrEqualTo(1).WithMessage("Price must be more than 1");
            RuleFor(i => i.Quantity).GreaterThanOrEqualTo(1).WithMessage("Quantity must be more than 1");
            RuleFor(i => i.Location).Length(5, 100).WithMessage("Length must be between 5 and 100");
        }
    }
}
