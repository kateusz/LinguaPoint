using LinguaPoint.Orders.Infrastructure.Persistence;
using LinguaPoint.Shared.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinguaPoint.Orders.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrders(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureSqlServer<OrdersContext>(configuration);
        //services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}