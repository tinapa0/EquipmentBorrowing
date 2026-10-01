using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.StudentId);
        builder.Property(s => s.FullName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.StudentNumber).IsRequired().HasMaxLength(20);
        builder.HasIndex(s => s.StudentNumber).IsUnique();
    }
}