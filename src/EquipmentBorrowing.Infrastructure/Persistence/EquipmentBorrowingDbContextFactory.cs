using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EquipmentBorrowing.Infrastructure.Persistence;

public class EquipmentBorrowingDbContextFactory : IDesignTimeDbContextFactory<EquipmentBorrowingDbContext>
{
	public EquipmentBorrowingDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<EquipmentBorrowingDbContext>();

		// Configures the DbContext to use SQLite for migrations
		optionsBuilder.UseSqlite("Data Source=equipment_borrowing.db");

		return new EquipmentBorrowingDbContext(optionsBuilder.Options);
	}
}