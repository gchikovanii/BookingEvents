using ItAcademy.Domain.BaseEntity;
using ItAcademy.Domain.EventsAggregate;
using ItAcademy.Domain.UserAggregate;

namespace ItAcademy.Domain.OrderAggregate
{
    public class Order : Entity
    {
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public decimal Price { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
