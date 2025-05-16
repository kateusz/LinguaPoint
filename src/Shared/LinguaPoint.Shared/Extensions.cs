using LinguaPoint.Shared.Commands;
using LinguaPoint.Shared.Events;
using LinguaPoint.Shared.Queries;
using LinguaPoint.Shared.Types.Kernel;
using Microsoft.Extensions.DependencyInjection;

namespace LinguaPoint.Shared;

public static class Extensions
{
    public static IServiceCollection AddFramework(this IServiceCollection services)
    {
        services.AddCommands();
        services.AddEvents();
        services.AddQueries();
        
        return services;
    }
}