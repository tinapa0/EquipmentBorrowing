using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<Student> _students = new()
    {
        new Student(1, "Alice", isAllowedToBorrow: true, maxAllowedBorrowings: 2),
        new Student(2, "Bob", isAllowedToBorrow: false, maxAllowedBorrowings: 2)
    };

    public Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(_students.FirstOrDefault(s => s.Id == id));
}

public class InMemoryEquipmentRepository : IEquipmentRepository
{
    private readonly List<Equipment> _equipmentList = new()
    {
        new Equipment(101, "Oscilloscope", isAvailable: true),
        new Equipment(102, "Digital Multimeter", isAvailable: false)
    };

    public Task<Equipment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(_equipmentList.FirstOrDefault(e => e.Id == id));

    public Task UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

public class InMemoryBorrowingRepository : IBorrowingRepository
{
    private readonly List<Borrowing> _borrowings = new();

    public Task AddAsync(Borrowing borrowing, CancellationToken cancellationToken = default)
    {
        _borrowings.Add(borrowing);
        return Task.CompletedTask;
    }

    public Task<int> GetActiveCountByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        => Task.FromResult(_borrowings.Count(b => b.StudentId == studentId && b.Status == BorrowingStatus.Active));
}