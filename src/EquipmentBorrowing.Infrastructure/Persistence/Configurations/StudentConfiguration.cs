using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.StudentNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(s => s.StudentNumber).IsUnique();

        builder.Property(s => s.FullName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.IsAllowedToBorrow).IsRequired();
        builder.Property(s => s.MaxAllowedBorrowings).IsRequired();

        builder.HasData(
            new { Id = 1, StudentNumber = "2026-0001", FullName = "Maria Kristina Sedeno", IsAllowedToBorrow = true, MaxAllowedBorrowings = 3 },
            new { Id = 2, StudentNumber = "2026-0002", FullName = "Ritch Vaughn Agne", IsAllowedToBorrow = true, MaxAllowedBorrowings = 3 }
        );
    }
}