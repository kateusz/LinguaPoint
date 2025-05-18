using LinguaPoint.Shared.Types.Kernel;
using LinguaPoint.Shared.Types.Kernel.Types;
using Microsoft.EntityFrameworkCore;

namespace LinguaPoint.Marketplace.Infrastructure.Database;


internal class MarketplaceContext : DbContext
{
    private readonly IDomainEventDispatcher? _domainEventDispatcher;
    
    public MarketplaceContext(DbContextOptions<MarketplaceContext> options) : base(options)
    {
    }

    public override int SaveChanges()
    {
        //UpdateVersions();

        return base.SaveChanges();
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("marketplace");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
    
    // TODO:
    // private void UpdateVersions()
    // {
    //     foreach (var change in ChangeTracker.Entries())
    //     {
    //         try
    //         {
    //             var versionProp = change.Member(nameof(DbModel.Version));
    //             versionProp.CurrentValue = Guid.NewGuid();
    //             
    //             if (change.State == EntityState.Added)
    //             {
    //                 var createDateProp = change.Member(nameof(DbModel.CreateDateUtc));
    //                 createDateProp.CurrentValue = DateTime.UtcNow;   
    //             }
    //             else
    //             {
    //                 var updateDateProp = change.Member(nameof(DbModel.UpdateDateUtc));
    //                 updateDateProp.CurrentValue = DateTime.UtcNow;
    //             }
    //         } catch { }
    //     }
    // }
}