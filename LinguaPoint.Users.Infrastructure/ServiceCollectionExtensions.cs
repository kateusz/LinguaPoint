using LinguaPoint.Shared.Database;
using LinguaPoint.Users.Core.Repositories;
using LinguaPoint.Users.Infrastructure.Persistence;
using LinguaPoint.Users.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinguaPoint.Users.Infrastructure;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUsers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureSqlServer<UsersContext>(configuration);
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}