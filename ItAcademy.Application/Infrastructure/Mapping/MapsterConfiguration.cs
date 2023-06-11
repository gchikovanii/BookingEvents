using ItAcademy.Application.Events.Response;
using ItAcademy.Application.Orders.Responses;
using ItAcademy.Domain.EventsAggregate;
using ItAcademy.Domain.OrderAggregate;
using Mapster;
using Microsoft.Extensions.DependencyInjection;


namespace ItAcademy.Application.Infrastructure.Mapping
{
    public static class MapsterConfiguration
    {
        public static void AddMaps(this IServiceCollection services)
        {
            TypeAdapterConfig<Event, EventResponse>
               .NewConfig()
               .Map(i => i.Email, o => o.User.Email);
            TypeAdapterConfig<Order, OrderResponse>.NewConfig();
        }
    }
}
