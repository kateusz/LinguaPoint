using LinguaPoint.Orders.Domain.Orders;
using LinguaPoint.Shared.Types.Kernel;
using LinguaPoint.Shared.Types.Kernel.Types;
using Microsoft.EntityFrameworkCore;

namespace LinguaPoint.Orders.Infrastructure.Persistence;

public class OrdersContext : DbContext
{
    private readonly IDomainEventDispatcher? _domainEventDispatcher;

    public DbSet<TranslationOrder> TranslationOrders { get; set; } = null!;

    public OrdersContext(DbContextOptions<OrdersContext> options, IDomainEventDispatcher? domainEventDispatcher = null) 
        : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("orders");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events before saving changes
        if (_domainEventDispatcher != null)
        {
            var entitiesWithEvents = ChangeTracker.Entries<AggregateRoot>()
                .Select(e => e.Entity)
                .Where(e => e.Events.Any())
                .ToArray();

            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.Events.ToArray();
                entity.ClearEvents();
                
                foreach (var domainEvent in events)
                {
                    await _domainEventDispatcher.DispatchAsync(domainEvent, cancellationToken);
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}