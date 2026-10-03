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
        var student = await _studentRepository.GetByIdAsync(studentId);
        var equipment = await _equipmentRepository.GetByIdAsync(equipmentId);

        if (student == null) throw new Exception("Selected student was not found.");
        if (equipment == null) throw new Exception("Selected equipment was not found.");
        if (!equipment.IsAvailable) throw new Exception("Equipment is currently borrowed.");

        equipment.MarkAsBorrowed();
        await _equipmentRepository.UpdateAsync(equipment);

        var borrowing = new Borrowing(
            0,
            studentId,
            equipmentId,
            expectedReturnDate ?? DateTime.UtcNow.AddDays(7)
        )
        {
            Student = student,
            Equipment = equipment
        };

        await _borrowingRepository.AddAsync(borrowing);
    }
}