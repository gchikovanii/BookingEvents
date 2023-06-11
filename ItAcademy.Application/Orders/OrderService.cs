using ItAcademy.Application.Orders.Repositories;
using ItAcademy.Application.Orders.Requests;
using ItAcademy.Application.Orders.Responses;
using ItAcademy.Domain.OrderAggregate;
using Mapster;

namespace ItAcademy.Application.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<IEnumerable<OrderResponse>> GetAllOrders(CancellationToken token, string userName)
        {
            var order = await _orderRepository.GetAllOrders(token, userName).ConfigureAwait(false);
            return order.Adapt<IEnumerable<OrderResponse>>();
        }
       
        public async Task<Order> GetOrder(CancellationToken token, int id)
        {
            return await _orderRepository.GetOrder(token, id).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersForAdmin(CancellationToken token)
        {
            var order = await _orderRepository.GetAllOrdersForAdmin(token).ConfigureAwait(false);
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrders(CancellationToken token, string userId, string userRole)
        {
            var result = await _orderRepository.GetAllOrdersByIdAndRole(token,userId, userRole).ConfigureAwait(false);
            return result;
        }

        public async Task<bool> MakeOrder(CancellationToken token, OrderRequest order, int eventId)
        {
            return await _orderRepository.AddOrder(token, order, eventId).ConfigureAwait(false);
        }
    }
}
