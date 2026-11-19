using System;

namespace EquipmentBorrowing.Domain;

public class Student
{
    public int Id { get; private set; }
    public string StudentNumber { get; private set; } = string.Empty; // BR-01: Required, unique institutional number
    public string FullName { get; private set; } = string.Empty;       // BR-02: Required full name
    public string? ContactInfo { get; private set; }                  // BR-02: Optional contact info

    public bool IsAllowedToBorrow { get; private set; } = true;       // BR-04: Eligibility status
    public int MaxAllowedBorrowings { get; set; } = 3;                // BR-09: Max limit

    protected Student() { } // Required for EF Core

    public Student(int id, string studentNumber, string fullName, string? contactInfo = null, bool isAllowedToBorrow = true, int maxAllowedBorrowings = 3)
    {
        Id = id;
        SetStudentNumber(studentNumber);
        SetFullName(fullName);
        SetContactInfo(contactInfo);
        IsAllowedToBorrow = isAllowedToBorrow;
        MaxAllowedBorrowings = maxAllowedBorrowings;
    }

    public void SetStudentNumber(string studentNumber)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
            throw new ArgumentException("Student number cannot be blank.", nameof(studentNumber));
        StudentNumber = studentNumber.Trim(); // BR-01: Trim leading/trailing whitespace
    }

    public void SetFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be blank.", nameof(fullName));
        FullName = fullName.Trim();
    }

    public void SetContactInfo(string? contactInfo)
    {
        // BR-02 / BR-03: Blank optional contact is stored as null
        ContactInfo = string.IsNullOrWhiteSpace(contactInfo) ? null : contactInfo.Trim();
    }

    public void Permit() => IsAllowedToBorrow = true;
    public void Restrict() => IsAllowedToBorrow = false;
}