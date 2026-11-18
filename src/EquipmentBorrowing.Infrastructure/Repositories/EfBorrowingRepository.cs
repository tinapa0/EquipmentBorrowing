using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class EfBorrowingRepository : IBorrowingRepository
{
    private readonly IDbContextFactory<EquipmentBorrowingDbContext> _contextFactory;

    public EfBorrowingRepository(IDbContextFactory<EquipmentBorrowingDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<int> GetActiveCountByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Borrowings
            .CountAsync(b => b.StudentId == studentId && b.Status == BorrowingStatus.Active, cancellationToken);
    }

    public async Task<IEnumerable<Borrowing>> GetActiveBorrowingsAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Borrowings
            .AsNoTracking()
            .Include(b => b.Equipment)
            .Include(b => b.Student)
            .Where(b => b.Status == BorrowingStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<Borrowing?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Borrowings
            .Include(b => b.Equipment)
            .Include(b => b.Student)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task AddAsync(Borrowing borrowing, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await context.Borrowings.AddAsync(borrowing, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Borrowing borrowing, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        context.Borrowings.Update(borrowing);
        await context.SaveChangesAsync(cancellationToken);
    }
}