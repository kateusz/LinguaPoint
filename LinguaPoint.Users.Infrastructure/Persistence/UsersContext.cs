using LinguaPoint.Shared.Types.Kernel;
using LinguaPoint.Users.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LinguaPoint.Users.Infrastructure.Persistence;

public class UsersContext : DbContext
{
    private readonly IDomainEventDispatcher? _domainEventDispatcher;

    public DbSet<User> Users { get; set; } = null!;

    public UsersContext(DbContextOptions<UsersContext> options, IDomainEventDispatcher? domainEventDispatcher = null) 
        : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events before saving changes
        if (_domainEventDispatcher != null)
        {
            var entitiesWithEvents = ChangeTracker.Entries<User>()
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