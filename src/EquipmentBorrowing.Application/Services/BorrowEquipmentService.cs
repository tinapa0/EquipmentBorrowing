using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
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

    // Support single checkout via overload for backward compatibility
    public async Task ExecuteAsync(int studentId, int equipmentId, DateTime? expectedReturnDate)
    {
        await ExecuteAsync(studentId, new List<int> { equipmentId }, expectedReturnDate);
    }

    // Multi-Item Checkout (UC-06)
    public async Task ExecuteAsync(int studentId, IEnumerable<int> equipmentIds, DateTime? expectedReturnDate)
    {
        var equipmentList = equipmentIds.ToList();
        if (!equipmentList.Any())
            throw new InvalidOperationException("No equipment items selected for checkout.");

        // 1. BR-08: Verify student exists and is eligible
        var student = await _studentRepository.GetByIdAsync(studentId)
            ?? throw new InvalidOperationException("Selected student was not found.");

        if (!student.IsAllowedToBorrow)
            throw new InvalidOperationException("Student is not eligible to borrow equipment.");

        // 2. BR-09: Enforce maximum limit considering active borrowings + requested items
        int activeCount = await _borrowingRepository.GetActiveCountByStudentIdAsync(studentId);
        int maxAllowed = student.MaxAllowedBorrowings;

        if (activeCount + equipmentList.Count > maxAllowed)
            throw new InvalidOperationException($"Checkout exceeds limit. Student has {activeCount} active borrowings, trying to check out {equipmentList.Count} more (Max allowed: {maxAllowed}).");

        // 3. BR-10: Verify all selected equipment items exist and are available
        var validatedEquipment = new List<Equipment>();
        foreach (var eqId in equipmentList)
        {
            var equipment = await _equipmentRepository.GetByIdAsync(eqId)
                ?? throw new InvalidOperationException($"Equipment with ID {eqId} was not found.");

            if (!equipment.IsAvailable)
                throw new InvalidOperationException($"Equipment '{equipment.Name}' ({equipment.EquipmentIdentifier}) is currently unavailable.");

            validatedEquipment.Add(equipment);
        }

        // 4. BR-11: Verify expected return date is in the future
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

        // 5. Process multi-item checkout atomically
        foreach (var equipment in validatedEquipment)
        {
            equipment.MarkAsBorrowed();
            await _equipmentRepository.UpdateAsync(equipment);

            var borrowing = new Borrowing(
                0,
                studentId,
                equipment.Id,
                returnDate
            );

            await _borrowingRepository.AddAsync(borrowing);
        }
    }
}