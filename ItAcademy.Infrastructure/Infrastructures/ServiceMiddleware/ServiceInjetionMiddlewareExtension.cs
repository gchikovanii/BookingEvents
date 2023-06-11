using ItAcademy.Application.Accounts.Repositories;
using ItAcademy.Application.Accounts;
using ItAcademy.Application.Events.Repositories;
using ItAcademy.Application.Events;
using ItAcademy.Application.Manage.Repositories;
using ItAcademy.Application.Manage;
using ItAcademy.Application.Orders.Repositories;
using ItAcademy.Application.Orders;
using Microsoft.Extensions.DependencyInjection;
using ItAcademy.Infrastructure.BaseRepo;
using ItAcademy.Infrastructure.Users;
using ItAcademy.Infrastructure.Events;
using ItAcademy.Infrastructure.Orders;
using ItAcademy.Infrastructure.Manage;
using ItAcademy.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;
using ItAcademy.Application.Accounts.Helper;

namespace ItAcademy.Infrastructure.Infrastructures.ServiceMiddleware
{
    public static class ServiceInjetionMiddlewareExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IManageTimeService, ManageTimeService>();
            services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher>();

            #region AddRepos
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IManageTimeRepository, ManageTimeRepository>();
            services.AddScoped<IReserveRepository, ReserveRepository>();
            #endregion

        }
    }
}
