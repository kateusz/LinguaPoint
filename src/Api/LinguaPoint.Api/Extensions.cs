using LinguaPoint.Orders.Api;
using LinguaPoint.Orders.Infrastructure;
using LinguaPoint.Shared;
using LinguaPoint.Users.Api;
using LinguaPoint.Users.Infrastructure;

namespace LinguaPoint.Api;

internal static class Extensions
{
    public static IServiceCollection AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddFramework()
            .AddModules(configuration)
            .AddHttpContextAccessor();

        //services.AddTransient<ExceptionHandlingMiddleware>();
        //services.AddTransient<NaiveAccessControlMiddleware>();
        
        //services.AddScoped<IUserContextAccessor, NaiveUserContextAccessor>();

        return services;
    }

    private static IServiceCollection AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddUsers(configuration);
        services.AddOrders(configuration); 
        return services;
    }

    public static IEndpointRouteBuilder RegisterModules(this IEndpointRouteBuilder builder)
    {
        builder.RegisterUserEndpoints();
        builder.RegisterOrderEndpoints(); 

        return builder;
    }
}