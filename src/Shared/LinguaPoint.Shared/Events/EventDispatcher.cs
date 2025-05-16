using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LinguaPoint.Shared.Events;

internal sealed class EventDispatcher : IEventDispatcher, IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly ConcurrentQueue<IEvent> _eventsToDispatch = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private readonly ILogger<EventDispatcher> _logger;
    private Timer _timer;

    public EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    private async void DoWork(object? state)
    {
        try
        {
            await _semaphore.WaitAsync();
            var dispatchTasks = new List<Task>();
            while (_eventsToDispatch.TryDequeue(out var @event))
            {
                dispatchTasks.Add(DispatchAsync(@event));
            }
            await Task.WhenAll(dispatchTasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
    
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        _eventsToDispatch.Enqueue(@event);
        return Task.CompletedTask;
    }
    
    private async Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        var eventJson = JsonSerializer.Serialize(@event);
        _logger.LogInformation("[EVENT DISPATCH]" + Environment.NewLine + @event.GetType().Name + Environment.NewLine + eventJson);
        
        if (typeof(IEvent).IsAssignableFrom(typeof(TEvent)))
        {
            await DispatchDynamicallyAsync(@event, cancellationToken);
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();
        var tasks = handlers.Select(x => x.HandleAsync(@event, cancellationToken));
        await Task.WhenAll(tasks);
    }
    
    private async Task DispatchDynamicallyAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
        var handlers = scope.ServiceProvider.GetServices(handlerType);
        var method = handlerType.GetMethod(nameof(IEventHandler<IEvent>.HandleAsync));
        if (method is null)
        {
            throw new InvalidOperationException($"Event handler for '{@event.GetType().Name}' is invalid.");
        }

        var tasks = handlers.Select(x => (Task)method.Invoke(x, new object[] { @event, cancellationToken }));
        await Task.WhenAll(tasks);
    }
}