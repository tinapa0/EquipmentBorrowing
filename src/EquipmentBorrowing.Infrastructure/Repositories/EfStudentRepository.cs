using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class EfStudentRepository : IStudentRepository
{
    private readonly IDbContextFactory<EquipmentBorrowingDbContext> _contextFactory;

    public EfStudentRepository(IDbContextFactory<EquipmentBorrowingDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<Student>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Students
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Students
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}