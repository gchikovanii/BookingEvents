using System.ComponentModel.DataAnnotations;

namespace ItAcademy.Application.Events.Dtos
{
    public class EventDto
    {
        public int Id { get; set; }
        public bool Status { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is required")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Must be between 5 and 30")]
        public string Title { get; set; }
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Must be between 50 and 1000")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }
        [Display(Name = "Location")]
        [Required(ErrorMessage = "Location is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Must be between 5 and 50")]
        public string Location { get; set; }
        public bool Approved { get; set; }
        public string Email { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Image { get; set; }
    }
}
