using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
{
    public void Configure(EntityTypeBuilder<Borrowing> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.StudentId)
               .IsRequired();

        builder.Property(b => b.EquipmentId)
               .IsRequired();

        builder.Property(b => b.BorrowedDate)
               .IsRequired();

        builder.Property(b => b.ExpectedReturnDate)
               .IsRequired();

        builder.Property(b => b.Status)
               .HasConversion<string>()
               .IsRequired();

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