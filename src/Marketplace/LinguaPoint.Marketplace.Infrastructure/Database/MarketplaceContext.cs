using Microsoft.EntityFrameworkCore;

namespace LinguaPoint.Marketplace.Infrastructure.Database;


internal class MarketplaceContext : DbContext
{
    public MarketplaceContext(DbContextOptions<MarketplaceContext> options) : base(options)
    {
    }

    public override int SaveChanges()
    {
        //UpdateVersions();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        //UpdateVersions();
        
        return base.SaveChangesAsync(cancellationToken);
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