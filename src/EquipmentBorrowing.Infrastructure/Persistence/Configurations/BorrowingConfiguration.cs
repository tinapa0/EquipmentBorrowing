using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
{
    public void Configure(EntityTypeBuilder<Borrowing> builder)
    {
        builder.HasKey(b => b.BorrowingId);

        builder.HasOne<Student>()
               .WithMany()
               .HasForeignKey(b => b.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Equipment>()
               .WithMany()
               .HasForeignKey(b => b.EquipmentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}