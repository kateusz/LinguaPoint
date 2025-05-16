using LinguaPoint.Orders.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinguaPoint.Orders.Infrastructure.Persistence.Configurations;

internal class TranslationFileConfiguration : IEntityTypeConfiguration<TranslationFile>
{
    public void Configure(EntityTypeBuilder<TranslationFile> builder)
    {
        builder.ToTable("TranslationFiles");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();
            
        builder.Property(x => x.OrderId);
        builder.Property(x => x.FileName)
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(x => x.FilePath)
            .HasMaxLength(1024)
            .IsRequired();
            
        builder.Property(x => x.FileStatus)
            .HasConversion<string>();
            
        builder.Property(x => x.Error)
            .HasMaxLength(2000);
            
        builder.Property(x => x.TranslatedFilePath)
            .HasMaxLength(1024);
            
        builder.Property(x => x.ReviewStatus)
            .HasConversion<string>();
            
        builder.Property(x => x.RevisionComment)
            .HasMaxLength(2000);
            
        // Created computed column for IsDelivered based on the TranslatedFilePath
        builder.Property(x => x.IsDelivered)
            .HasComputedColumnSql("CASE WHEN [TranslatedFilePath] IS NOT NULL AND LEN([TranslatedFilePath]) > 0 THEN 1 ELSE 0 END");
    }
}