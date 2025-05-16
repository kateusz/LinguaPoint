using LinguaPoint.Shared.Database;
using LinguaPoint.Users.Core.Repositories;
using LinguaPoint.Users.Core.Services;
using LinguaPoint.Users.Infrastructure.Persistence;
using LinguaPoint.Users.Infrastructure.Repositories;
using LinguaPoint.Users.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinguaPoint.Users.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUsers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureSqlServer<UsersContext>(configuration);
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        
        return services;
    }
}