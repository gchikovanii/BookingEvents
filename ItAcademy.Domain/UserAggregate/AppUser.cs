using System.ComponentModel.DataAnnotations;
using ItAcademy.Domain.EventsAggregate;
using ItAcademy.Domain.OrderAggregate;
using Microsoft.AspNetCore.Identity;

namespace ItAcademy.Domain.UserAggregate
{
    public class AppUser : IdentityUser
    {
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        public bool Status { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
