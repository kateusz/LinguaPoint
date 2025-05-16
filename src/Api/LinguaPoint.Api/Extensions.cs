using LinguaPoint.Shared;
using LinguaPoint.Shared.Database;
using LinguaPoint.Shared.UserContext;
using LinguaPoint.Users.Infrastructure;

namespace LinguaPoint.Api;

internal static class Extensions
{
    public static void AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddModules(configuration);
        services.AddFramework();
        services.AddUsers(configuration);
        
        //services.AddTransient<ExceptionHandlingMiddleware>();
        //services.AddTransient<NaiveAccessControlMiddleware>();
        services.AddHttpContextAccessor();
        //services.AddScoped<IUserContextAccessor, NaiveUserContextAccessor>();
    }
}