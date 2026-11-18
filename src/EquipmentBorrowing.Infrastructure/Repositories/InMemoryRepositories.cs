using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<Student> _students = new()
    {
        new Student(1, "2026-0001", "Alice", isAllowedToBorrow: true, maxAllowedBorrowings: 2),
        new Student(2, "2026-0002", "Bob", isAllowedToBorrow: false, maxAllowedBorrowings: 2)
    };

    public Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(_students.FirstOrDefault(s => s.Id == id));

    public Task<IEnumerable<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Student>>(_students);

    public Task AddAsync(Student student, CancellationToken cancellationToken = default)
    {
        _students.Add(student);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Student student, CancellationToken cancellationToken = default)
    {
        var idx = _students.FindIndex(s => s.Id == student.Id);
        if (idx >= 0)
            _students[idx] = student;
        return Task.CompletedTask;
    }
}

public class InMemoryEquipmentRepository : IEquipmentRepository
{
    private readonly List<Equipment> _equipmentList = new()
    {
        new Equipment(101, "EQ-101", "Oscilloscope", isAvailable: true),
        new Equipment(102, "EQ-102", "Digital Multimeter", isAvailable: false)
    };

    public Task<Equipment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(_equipmentList.FirstOrDefault(e => e.Id == id));

    public Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Equipment>>(_equipmentList);

    public Task AddAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        _equipmentList.Add(equipment);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        var idx = _equipmentList.FindIndex(e => e.Id == equipment.Id);
        if (idx >= 0) _equipmentList[idx] = equipment;
        return Task.CompletedTask;
    }
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

    public Task<Borrowing?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(_borrowings.FirstOrDefault(b => b.Id == id));

    public Task UpdateAsync(Borrowing borrowing, CancellationToken cancellationToken = default)
    {
        var idx = _borrowings.FindIndex(b => b.Id == borrowing.Id);
        if (idx >= 0) _borrowings[idx] = borrowing;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Borrowing>> GetActiveBorrowingsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Borrowing>>(_borrowings.Where(b => b.Status == BorrowingStatus.Active).ToList());

    public async Task<IEnumerable<Borrowing>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return _borrowings.Where(b => b.StudentId == studentId);
    }

    public async Task<IEnumerable<Borrowing>> GetByEquipmentIdAsync(int equipmentId, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return _borrowings.Where(b => b.EquipmentId == equipmentId);
    }

    public async Task<IEnumerable<Borrowing>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return _borrowings;
    }
}
