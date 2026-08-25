using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Infrastructure.Repositories;

var studentRepo = new InMemoryStudentRepository();
var equipmentRepo = new InMemoryEquipmentRepository();
var borrowingRepo = new InMemoryBorrowingRepository();

var borrowService = new BorrowEquipmentService(studentRepo, equipmentRepo, borrowingRepo);

Console.WriteLine("--- SUCCESS CASE DEMO ---");
try
{
    var borrowing = await borrowService.ExecuteAsync(studentId: 1, equipmentId: 101);
    Console.WriteLine($"SUCCESS: Borrowing created with ID #{borrowing.Id} for Equipment #{borrowing.EquipmentId}");
}
catch (Exception ex)
{
    Console.WriteLine($"FAILED: {ex.Message}");
}

Console.WriteLine("\n--- FAILURE CASE DEMO (Unavailable Equipment) ---");
try
{
    await borrowService.ExecuteAsync(studentId: 1, equipmentId: 102);
}
catch (Exception ex)
{
    Console.WriteLine($"EXPECTED FAILURE: {ex.Message}");
}

Console.WriteLine("\n--- FAILURE CASE DEMO (Disallowed Student) ---");
try
{
    await borrowService.ExecuteAsync(studentId: 2, equipmentId: 101);
}
catch (Exception ex)
{
    Console.WriteLine($"EXPECTED FAILURE: {ex.Message}");
}