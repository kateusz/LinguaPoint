using LinguaPoint.Orders.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinguaPoint.Orders.Infrastructure.Persistence.Configurations;

internal class TranslationOrderConfiguration : IEntityTypeConfiguration<TranslationOrder>
{
    public void Configure(EntityTypeBuilder<TranslationOrder> builder)
    {
        builder.ToTable("TranslationOrders");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasConversion(
                x => x.Value,
                x => new Shared.Types.Kernel.Types.AggregateId(x));
            
        builder.Property(x => x.ClientId);
        builder.Property(x => x.Version);
        builder.Property(x => x.Status)
            .HasConversion<string>();
        builder.Property(x => x.CreatedAt);
            
        // Value objects need to be stored as separate properties
        builder.OwnsOne(x => x.LanguagePair, lpBuilder =>
        {
            lpBuilder.Property(lp => lp.SourceLanguage)
                .HasColumnName("SourceLanguage");
                
            lpBuilder.Property(lp => lp.TargetLanguage)
                .HasColumnName("TargetLanguage");
        });
        
        // Money is a value object
        builder.OwnsOne(x => x.Price, priceBuilder =>
        {
            priceBuilder.Property(p => p.Amount)
                .HasColumnName("Price_Amount")
                .HasColumnType("decimal(18,2)");
                
            priceBuilder.Property(p => p.Currency)
                .HasColumnName("Price_Currency")
                .HasMaxLength(3);
        });
        
        // Navigation properties
        builder.HasMany(x => x.Files)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Owned entity collection (EF Core 5.0+)
        builder.OwnsMany(x => x.Offers, offersBuilder =>
        {
            offersBuilder.ToTable("TranslationOffers");
            
            offersBuilder.WithOwner().HasForeignKey("OrderId");
            
            offersBuilder.HasKey(x => x.Id);
            
            offersBuilder.Property(x => x.TranslatorId);
            offersBuilder.Property(x => x.CreatedAt);
            
            // Price value object
            offersBuilder.OwnsOne(x => x.Price, priceBuilder =>
            {
                priceBuilder.Property(p => p.Amount)
                    .HasColumnName("Price_Amount")
                    .HasColumnType("decimal(18,2)");
                    
                priceBuilder.Property(p => p.Currency)
                    .HasColumnName("Price_Currency")
                    .HasMaxLength(3);
            });
        });
        
        // Shadow property to store the accepted offer ID
        builder.Property<Guid?>("AcceptedOfferId");
    }
}