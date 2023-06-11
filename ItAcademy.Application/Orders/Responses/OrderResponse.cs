namespace ItAcademy.Application.Orders.Responses
{
    public class OrderResponse
    {
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public decimal Price { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
    }
}
