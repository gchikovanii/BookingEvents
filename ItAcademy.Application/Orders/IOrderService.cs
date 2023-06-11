using ItAcademy.Application.Orders.Requests;
using ItAcademy.Application.Orders.Responses;
using ItAcademy.Domain.OrderAggregate;

namespace ItAcademy.Application.Orders
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponse>> GetAllOrders(CancellationToken token, string userName);
        Task<Order> GetOrder(CancellationToken token, int id);
        Task<bool> MakeOrder(CancellationToken token, OrderRequest order, int eventId);
        Task<IEnumerable<Order>> GetAllOrdersForAdmin(CancellationToken token);
        Task<IEnumerable<Order>> GetOrders(CancellationToken token, string userId, string userRole);
    }
}
