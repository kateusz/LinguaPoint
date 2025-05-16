namespace LinguaPoint.Shared.Types.Kernel;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IDomainEvent @event, CancellationToken cancellationToken = default);
    Task DispatchAsync(IDomainEvent[] events, CancellationToken cancellationToken = default);
}