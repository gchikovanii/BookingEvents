using ItAcademy.Application.Orders.Requests;
using ItAcademy.Domain.OrderAggregate;

namespace ItAcademy.Application.Orders.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrders(CancellationToken token, string userName);
        Task<IEnumerable<Order>> GetAllOrdersForAdmin(CancellationToken token);
        Task<Order> GetOrder(CancellationToken token, int id);
        Task<bool> AddOrder(CancellationToken token, OrderRequest order, int eventId);
        Task<List<Order>> GetAllOrdersByIdAndRole(CancellationToken token, string userId, string userRole);
    }
}
