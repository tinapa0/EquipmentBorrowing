using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EquipmentBorrowing.Domain;

public class Borrowing
{
    public int Id { get; private set; }

    public int StudentId { get; private set; }
    [ForeignKey(nameof(StudentId))]
    public Student? Student { get; set; }

    public int EquipmentId { get; private set; }
    [ForeignKey(nameof(EquipmentId))]
    public Equipment? Equipment { get; set; }

    public DateTime BorrowedDate { get; private set; }
    public DateTime ExpectedReturnDate { get; private set; }
    public BorrowingStatus Status { get; private set; }

    private Borrowing() { } // Required for EF Core

    public Borrowing(int id, int studentId, int equipmentId, DateTime expectedReturnDate)
    {
        Id = id;
        StudentId = studentId;
        EquipmentId = equipmentId;
        BorrowedDate = DateTime.UtcNow;
        ExpectedReturnDate = expectedReturnDate;
        Status = BorrowingStatus.Active;
    }

    public void MarkAsReturned() => Status = BorrowingStatus.Returned;
}