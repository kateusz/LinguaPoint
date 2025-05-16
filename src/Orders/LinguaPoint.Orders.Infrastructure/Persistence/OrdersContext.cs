using LinguaPoint.Orders.Domain.Orders;
using LinguaPoint.Shared.Types.Kernel;
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events before saving changes
        // TODO: FInish it

        return await base.SaveChangesAsync(cancellationToken);
    }
}