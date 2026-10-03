using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Infrastructure.Repositories;

var studentRepo = new InMemoryStudentRepository();
var equipmentRepo = new InMemoryEquipmentRepository();
var borrowingRepo = new InMemoryBorrowingRepository();

// 1. Fixed constructor parameter order: (IBorrowingRepository, IEquipmentRepository, IStudentRepository)
var borrowService = new BorrowEquipmentService(borrowingRepo, equipmentRepo, studentRepo);

Console.WriteLine("--- SUCCESS CASE DEMO ---");
try
{
    // 2. Added expectedReturnDate argument and removed return assignment (ExecuteAsync is a void Task)
    await borrowService.ExecuteAsync(studentId: 1, equipmentId: 101, expectedReturnDate: DateTime.UtcNow.AddDays(7));
    Console.WriteLine("SUCCESS: Borrowing created for Equipment #101");
}
catch (Exception ex)
{
    Console.WriteLine($"FAILED: {ex.Message}");
}

Console.WriteLine("\n--- FAILURE CASE DEMO (Unavailable Equipment) ---");
try
{
    await borrowService.ExecuteAsync(studentId: 1, equipmentId: 102, expectedReturnDate: DateTime.UtcNow.AddDays(7));
}
catch (Exception ex)
{
    Console.WriteLine($"EXPECTED FAILURE: {ex.Message}");
}

Console.WriteLine("\n--- FAILURE CASE DEMO (Disallowed Student) ---");
try
{
    await borrowService.ExecuteAsync(studentId: 2, equipmentId: 101, expectedReturnDate: DateTime.UtcNow.AddDays(7));
}
catch (Exception ex)
{
    Console.WriteLine($"EXPECTED FAILURE: {ex.Message}");
}