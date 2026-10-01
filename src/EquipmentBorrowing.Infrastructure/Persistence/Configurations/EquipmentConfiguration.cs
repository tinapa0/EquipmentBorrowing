using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.IsAvailable).IsRequired();

        builder.HasData(
            new { Id = 1, Name = "Projector HD-1080", IsAvailable = true },
            new { Id = 2, Name = "DSLR Camera Canon", IsAvailable = true },
            new { Id = 3, Name = "Wireless Microphone", IsAvailable = true }
        );
    }
}