using HRSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRSystem.Infrastructure.Persistence.Configurations;

public class ScreenResourceConfiguration : IEntityTypeConfiguration<ScreenResource>
{
    public void Configure(EntityTypeBuilder<ScreenResource> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.RoutePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.Icon)
            .HasMaxLength(100);

        builder.HasOne(s => s.ParentScreen)
            .WithMany(s => s.ChildScreens)
            .HasForeignKey(s => s.ParentScreenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.RoutePath);
    }
}