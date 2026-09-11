using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class ReturnEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IBorrowingRepository _borrowingRepository;

    public ReturnEquipmentService(IEquipmentRepository equipmentRepository, IBorrowingRepository borrowingRepository)
    {
        _equipmentRepository = equipmentRepository;
        _borrowingRepository = borrowingRepository;
    }

    public async Task<Borrowing> ExecuteAsync(int borrowingId)
    {
        var borrowing = await _borrowingRepository.GetByIdAsync(borrowingId)
            ?? throw new InvalidOperationException("Borrowing does not exist.");

        if (borrowing.Status == BorrowingStatus.Returned)
            throw new InvalidOperationException("Borrowing has already been returned.");

        var equipment = await _equipmentRepository.GetByIdAsync(borrowing.EquipmentId)
            ?? throw new InvalidOperationException("Equipment does not exist.");

        // Mark return on domain entities
        borrowing.MarkAsReturned();
        equipment.MarkAsReturned();

        await _borrowingRepository.UpdateAsync(borrowing);
        await _equipmentRepository.UpdateAsync(equipment);

        return borrowing;
    }
}
