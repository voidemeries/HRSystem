using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRSystem.Infrastructure.Persistence.Configurations;

public class PermissionAssignmentConfiguration : IEntityTypeConfiguration<PermissionAssignment>
{
    public void Configure(EntityTypeBuilder<PermissionAssignment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.ScopeType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.PermissionType)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(p => p.ScreenResource)
            .WithMany()
            .HasForeignKey(p => p.ScreenResourceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => new { p.ScreenResourceId, p.ScopeType, p.ScopeId, p.PermissionType })
            .IsUnique();
    }
}