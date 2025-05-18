using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace LinguaPoint.Shared.Commands;

internal static class Extensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        var assembly = Assembly.Load("LinguaPoint.Users.Core");
        
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
            
        return services;
    }
}