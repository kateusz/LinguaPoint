using LinguaPoint.Marketplace.Infrastructure.Database;
using LinguaPoint.Shared.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinguaPoint.Marketplace.Infrastructure;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMarketplace(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureSqlServer<MarketplaceContext>(configuration);
        return services;
    }
}