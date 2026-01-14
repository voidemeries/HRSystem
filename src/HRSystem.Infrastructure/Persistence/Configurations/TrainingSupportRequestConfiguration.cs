using HRSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRSystem.Infrastructure.Persistence.Configurations;

public class TrainingSupportRequestConfiguration : IEntityTypeConfiguration<TrainingSupportRequest>
{
    public void Configure(EntityTypeBuilder<TrainingSupportRequest> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RequestType).HasConversion<int>().IsRequired();
        builder.Property(r => r.Status).HasConversion<int>().IsRequired();
        builder.Property(r => r.TrainingName).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Provider).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Justification).IsRequired().HasMaxLength(1000);
        builder.Property(r => r.RejectionReason).HasMaxLength(1000);
        builder.Property(r => r.Cost).HasColumnType("decimal(18,2)");

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