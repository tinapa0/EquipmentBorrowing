using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class BorrowEquipmentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IBorrowingRepository _borrowingRepository;

    public BorrowEquipmentService(
        IStudentRepository studentRepository,
        IEquipmentRepository equipmentRepository,
        IBorrowingRepository borrowingRepository)
    {
        _studentRepository = studentRepository;
        _equipmentRepository = equipmentRepository;
        _borrowingRepository = borrowingRepository;
    }

    public async Task<Borrowing> ExecuteAsync(int studentId, int equipmentId, int durationDays = 7)
    {
        var student = await _studentRepository.GetByIdAsync(studentId)
            ?? throw new InvalidOperationException("Student does not exist.");

        if (!student.IsAllowedToBorrow)
            throw new InvalidOperationException("Student is not currently allowed to borrow equipment.");

        var equipment = await _equipmentRepository.GetByIdAsync(equipmentId)
            ?? throw new InvalidOperationException("Equipment does not exist.");

        if (!equipment.IsAvailable)
            throw new InvalidOperationException("Equipment is currently unavailable.");

        int activeCount = await _borrowingRepository.GetActiveCountByStudentIdAsync(studentId);
        if (activeCount >= student.MaxAllowedBorrowings)
            throw new InvalidOperationException("Student has reached the maximum allowed active borrowings.");

        equipment.MarkAsBorrowed();
        await _equipmentRepository.UpdateAsync(equipment);

        var borrowing = new Borrowing(Random.Shared.Next(1, 10000), student.Id, equipment.Id, DateTime.UtcNow.AddDays(durationDays));
        await _borrowingRepository.AddAsync(borrowing);

        return borrowing;
    }
}