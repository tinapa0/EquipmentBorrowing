using EquipmentBorrowing.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EquipmentBorrowing.Application.Interfaces;

public interface IBorrowingRepository
{
    Task AddAsync(Borrowing borrowing, CancellationToken cancellationToken = default);
    Task<Borrowing?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Borrowing borrowing, CancellationToken cancellationToken = default);
    Task<int> GetActiveCountByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Borrowing>> GetActiveBorrowingsAsync(CancellationToken cancellationToken = default);

    // Add these two for UC-07 and UC-08
    Task<IEnumerable<Borrowing>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Borrowing>> GetByEquipmentIdAsync(int equipmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Borrowing>> GetAllAsync(CancellationToken cancellationToken = default);
}