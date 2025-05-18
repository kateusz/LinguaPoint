using LinguaPoint.Shared.Types.Kernel.Types;
using LinguaPoint.Users.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinguaPoint.Users.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.Property(u => u.Id)
            .HasConversion(
                aggregateId => aggregateId.Value,      // To database (Guid)
                dbValue => new AggregateId(dbValue)    // From database to AggregateId
            );
        
        // Configure the Id property
        builder.HasKey(u => u.Id);
        
        // Ignore domain events
        builder.Ignore(u => u.Events);
        
        // Configure value objects
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => new Shared.Types.Kernel.ValueObjects.Email(value))
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(u => u.FullName)
            .HasConversion(
                name => name.Value,
                value => new Shared.Types.Kernel.ValueObjects.FullName(value))
            .HasMaxLength(100)
            .IsRequired();
        
        // Configure other properties
        builder.Property(u => u.PasswordHash)
            .IsRequired();
        
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(u => u.IsVerified)
            .IsRequired();
        
        builder.Property(u => u.IsActive)
            .IsRequired();
        
        builder.Property(u => u.CreatedAt)
            .IsRequired();
        
        builder.Property(u => u.LastLoginAt);

        // Configure relationships
        builder.OwnsMany(u => u.Languages, languageBuilder =>
        {
            languageBuilder.ToTable("UserLanguages");
            
            languageBuilder.WithOwner().HasForeignKey("UserId");
            
            languageBuilder.Property(l => l.LanguageCode)
                .HasMaxLength(10)
                .IsRequired();
            
            languageBuilder.Property(l => l.Proficiency)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
            
            languageBuilder.HasKey("UserId", "LanguageCode");
        });
    }
}