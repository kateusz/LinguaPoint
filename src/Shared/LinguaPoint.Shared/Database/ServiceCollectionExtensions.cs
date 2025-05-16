using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinguaPoint.Shared.Database;

public static class ServiceCollectionExtensions
{
    public static void EnsureDatabaseCreated(this IServiceProvider serviceProvider)
    {
        var dbContexts = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
            .Where(type => typeof(DbContext).IsAssignableFrom(type))
            .Where(type => !type.IsAbstract)
            .Where(type => type != typeof(DbContext))
            .ToList();

        using var scope = serviceProvider.CreateScope();
        
        var dbContextThatShouldMigrate = 
            dbContexts.Where(db => typeof(IDoNotMigrate).IsAssignableFrom(db) is false);
       
        // TODO
        // foreach (var context in dbContextThatShouldMigrate)
        // {
        //     (scope.ServiceProvider.GetService(context) as DbContext)?.Database.Migrate();   
        // }
    }
    
    public static IServiceCollection AddAzureSqlServer<T>(this IServiceCollection services, IConfiguration configuration) where T : DbContext
    {
        var connectionString = configuration.GetConnectionString("AzureSql");
        services.AddDbContext<T>(x => x.UseSqlServer(connectionString));
    
        return services;
    }
}