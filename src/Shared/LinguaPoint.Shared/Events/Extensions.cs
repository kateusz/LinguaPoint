﻿using LinguaPoint.Shared.Types.Kernel;
using Microsoft.Extensions.DependencyInjection;

namespace LinguaPoint.Shared.Events;

internal static class Extensions
{
    public static IServiceCollection AddEvents(this IServiceCollection services)
    {
        services.AddSingleton<IEventDispatcher, EventDispatcher>();
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddHostedService<EventDispatcher>();
        services.Scan(s => s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
            
        return services;
    }
}