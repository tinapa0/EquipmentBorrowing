using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using System;
using System.Threading.Tasks;

namespace EquipmentBorrowing.Application.Services;

public class BorrowEquipmentService
{
    private readonly IBorrowingRepository _borrowingRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IStudentRepository _studentRepository;

    public BorrowEquipmentService(
        IBorrowingRepository borrowingRepository,
        IEquipmentRepository equipmentRepository,
        IStudentRepository studentRepository)
    {
        _borrowingRepository = borrowingRepository;
        _equipmentRepository = equipmentRepository;
        _studentRepository = studentRepository;
    }

    public async Task ExecuteAsync(int studentId, int equipmentId, DateTime? expectedReturnDate)
    {
        // 1. BR-08: Verify student and equipment exist
        var student = await _studentRepository.GetByIdAsync(studentId)
            ?? throw new InvalidOperationException("Selected student was not found.");

        if (!student.IsAllowedToBorrow)
            throw new InvalidOperationException("Student is not eligible to borrow equipment.");

        var equipment = await _equipmentRepository.GetByIdAsync(equipmentId)
            ?? throw new InvalidOperationException("Selected equipment was not found.");

        // 2. BR-10: Verify equipment is available
        if (!equipment.IsAvailable)
            throw new InvalidOperationException("Equipment is currently unavailable.");

        // 3. BR-11: Verify expected return date is in the future
        DateTime returnDate = expectedReturnDate ?? DateTime.UtcNow.AddDays(7);

        // Force DateTimeKind to Utc so PostgreSQL accepts it
        if (returnDate.Kind == DateTimeKind.Unspecified)
        {
            returnDate = DateTime.SpecifyKind(returnDate, DateTimeKind.Utc);
        }
        else
        {
            returnDate = returnDate.ToUniversalTime();
        }

        if (returnDate.Date <= DateTime.UtcNow.Date)
            throw new InvalidOperationException("Expected return date must be later than today.");

        // 4. BR-09: Enforce maximum of 3 active borrowings per student
        int activeCount = await _borrowingRepository.GetActiveCountByStudentIdAsync(studentId);
        if (activeCount >= 3)
            throw new InvalidOperationException("Student has reached the maximum limit of 3 active borrowings.");

        // 5. Update equipment availability and save borrowing record
        equipment.MarkAsBorrowed();
        await _equipmentRepository.UpdateAsync(equipment);

        
        var borrowing = new Borrowing(
            0,
            studentId,
            equipmentId,
            returnDate
        );

        await _borrowingRepository.AddAsync(borrowing);
    }
}