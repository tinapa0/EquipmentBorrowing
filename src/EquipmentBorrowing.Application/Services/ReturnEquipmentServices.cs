using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EquipmentBorrowing.Application.Services;

public class ReturnEquipmentService
{
    private readonly IBorrowingRepository _borrowingRepository;
    private readonly IEquipmentRepository _equipmentRepository;

    public ReturnEquipmentService(
        IBorrowingRepository borrowingRepository,
        IEquipmentRepository equipmentRepository)
    {
        _borrowingRepository = borrowingRepository;
        _equipmentRepository = equipmentRepository;
    }

    public async Task ReturnAsync(int borrowingId, CancellationToken cancellationToken = default)
    {
        var borrowing = await _borrowingRepository.GetByIdAsync(borrowingId, cancellationToken)
            ?? throw new InvalidOperationException("Borrowing record not found.");

        if (borrowing.Status == BorrowingStatus.Returned)
            throw new InvalidOperationException("This equipment has already been returned.");

        var equipment = await _equipmentRepository.GetByIdAsync(borrowing.EquipmentId, cancellationToken)
            ?? throw new InvalidOperationException("Equipment record not found.");

        // Apply domain behavior logic
        borrowing.MarkAsReturned();

        // Use domain method if setter is encapsulated/private
        equipment.MarkAsReturned(); // was equipment.MarkAsAvailable();

        await _borrowingRepository.UpdateAsync(borrowing, cancellationToken);
        await _equipmentRepository.UpdateAsync(equipment, cancellationToken);
    }
}