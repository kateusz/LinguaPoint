using LinguaPoint.Shared;
using LinguaPoint.Shared.Database;
using LinguaPoint.Shared.UserContext;
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
        return services;
    }

    public static IEndpointRouteBuilder RegisterModules(this IEndpointRouteBuilder builder)
    {
        builder.RegisterUserEndpoints();

        return builder;
    }
}