using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EquipmentBorrowing.Infrastructure.Persistence;

public class EquipmentBorrowingDbContextFactory : IDesignTimeDbContextFactory<EquipmentBorrowingDbContext>
{
	public EquipmentBorrowingDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<EquipmentBorrowingDbContext>();

        // Configures the DbContext to use PostgreSQL for migrations
        optionsBuilder.UseNpgsql("Host=localhost;Database=equipment_borrowing_db;Username=postgres;Password=admin123");

        return new EquipmentBorrowingDbContext(optionsBuilder.Options);
	}
}