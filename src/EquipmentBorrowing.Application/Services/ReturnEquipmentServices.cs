using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;

namespace EquipmentBorrowing.Application.Services;

public class ReturnEquipmentService
{
    private readonly IBorrowingRepository _borrowingRepository;
    private readonly IEquipmentRepository _equipmentRepository;

    public ReturnEquipmentService(IBorrowingRepository borrowingRepository, IEquipmentRepository equipmentRepository)
    {
        _borrowingRepository = borrowingRepository;
        _equipmentRepository = equipmentRepository;
    }

    public async Task ReturnAsync(Guid borrowingId)
    {
        var borrowing = await _borrowingRepository.GetByIdAsync(borrowingId)
            ?? throw new InvalidOperationException("Borrowing record not found.");

        if (borrowing.ReturnedOn.HasValue)
            throw new InvalidOperationException("This equipment has already been returned.");

        var equipment = await _equipmentRepository.GetByIdAsync(borrowing.EquipmentId)
            ?? throw new InvalidOperationException("Equipment record not found.");

        borrowing.MarkAsReturned(DateTime.UtcNow);
        equipment.IsAvailable = true;

        await _borrowingRepository.UpdateAsync(borrowing);
        await _equipmentRepository.UpdateAsync(equipment);
    }
}