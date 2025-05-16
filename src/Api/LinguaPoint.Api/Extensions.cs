using LinguaPoint.Shared;
using LinguaPoint.Shared.Database;
using LinguaPoint.Shared.UserContext;

namespace Transload.Api;

internal static class Extensions
{
    public static void AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddModules(configuration);
        services.AddFramework();
        //services.AddTransient<ExceptionHandlingMiddleware>();
        //services.AddTransient<NaiveAccessControlMiddleware>();
        services.AddHttpContextAccessor();
        //services.AddScoped<IUserContextAccessor, NaiveUserContextAccessor>();
    }
}