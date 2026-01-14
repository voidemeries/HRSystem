using HRSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRSystem.Infrastructure.Persistence.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Code)
            .HasMaxLength(50);

        builder.HasOne(p => p.ParentPosition)
            .WithMany(p => p.ChildPositions)
            .HasForeignKey(p => p.ParentPositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.Code).IsUnique().HasFilter("[Code] IS NOT NULL");
    }
}