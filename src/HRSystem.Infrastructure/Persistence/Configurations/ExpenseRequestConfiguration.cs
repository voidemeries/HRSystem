using HRSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRSystem.Infrastructure.Persistence.Configurations;

public class ExpenseRequestConfiguration : IEntityTypeConfiguration<ExpenseRequest>
{
    public void Configure(EntityTypeBuilder<ExpenseRequest> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RequestType).HasConversion<int>().IsRequired();
        builder.Property(r => r.Status).HasConversion<int>().IsRequired();
        builder.Property(r => r.Category).IsRequired().HasMaxLength(100);
        builder.Property(r => r.Description).IsRequired().HasMaxLength(1000);
        builder.Property(r => r.RejectionReason).HasMaxLength(1000);
        builder.Property(r => r.Amount).HasColumnType("decimal(18,2)");

        builder.HasOne(r => r.Requester)
            .WithMany()
            .HasForeignKey(r => r.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ForEmployee)
            .WithMany()
            .HasForeignKey(r => r.ForEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ApproverPosition)
            .WithMany()
            .HasForeignKey(r => r.ApproverPositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Approver)
            .WithMany()
            .HasForeignKey(r => r.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}