using System.ComponentModel.DataAnnotations;
using ItAcademy.Domain.EventsAggregate;
using ItAcademy.Domain.UserAggregate;

namespace ItAcademy.Application.Orders.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Is Required")]

        public int Quantity { get; set; }
        [Required(ErrorMessage = "Is Required")]
        [Display(Name = "Total")]
        public decimal Total { get; set; }
        [Display(Name = "Total")]
        [Required(ErrorMessage ="Is Required")]
        public decimal Price { get; set; }
        public int EventId { get; set; }
        [Display(Name = "Event")]
        public Event Event { get; set; }
        public string UserId { get; set; }
        [Display(Name = "User")]
        public AppUser User { get; set; }
    }
}
