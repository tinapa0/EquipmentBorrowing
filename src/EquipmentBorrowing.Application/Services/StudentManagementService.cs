using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class StudentManagementService
{
    private readonly IStudentRepository _studentRepository;

    public StudentManagementService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<IReadOnlyList<Student>> GetAllStudentsAsync(CancellationToken cancellationToken = default)
    {
        var students = await _studentRepository.GetAllAsync(cancellationToken);
        return students.OrderBy(s => s.Id).ToList();
    }

    // UC-12: Register Student (BR-01, BR-02)
    public async Task<Student> RegisterStudentAsync(
        string studentNumber,
        string fullName,
        string? contactInfo = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
            throw new ArgumentException("Student number is required.");

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.");

        var normalizedNumber = studentNumber.Trim();
        var existingStudents = await _studentRepository.GetAllAsync(cancellationToken);

        if (existingStudents.Any(s => string.Equals(s.StudentNumber.Trim(), normalizedNumber, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A student with number '{normalizedNumber}' already exists.");

        var student = new Student(
            id: 0,
            studentNumber: normalizedNumber,
            fullName: fullName.Trim(),
            contactInfo: contactInfo,
            isAllowedToBorrow: true,
            maxAllowedBorrowings: 3);

        await _studentRepository.AddAsync(student, cancellationToken);
        return student;
    }

    // UC-13: Update Student Information (BR-01, BR-02, BR-03, BR-14)
    public async Task<Student> UpdateStudentInfoAsync(
        int studentId,
        string studentNumber,
        string fullName,
        string? contactInfo = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
            throw new ArgumentException("Student number is required.");

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.");

        var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken)
            ?? throw new InvalidOperationException("Selected student was not found.");

        var normalizedNumber = studentNumber.Trim();
        var existingStudents = await _studentRepository.GetAllAsync(cancellationToken);

        if (existingStudents.Any(s =>
            s.Id != studentId &&
            string.Equals(s.StudentNumber.Trim(), normalizedNumber, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Another student already uses number '{normalizedNumber}'.");
        }

        student.SetStudentNumber(normalizedNumber);
        student.SetFullName(fullName.Trim());
        student.SetContactInfo(contactInfo);

        await _studentRepository.UpdateAsync(student, cancellationToken);
        return student;
    }

    // UC-14: Change Student Borrowing Eligibility (BR-04, BR-14)
    public async Task<string> SetEligibilityAsync(
        int studentId,
        bool allowBorrowing,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken)
            ?? throw new InvalidOperationException("Selected student was not found.");

        if (student.IsAllowedToBorrow == allowBorrowing)
        {
            return allowBorrowing
                ? $"{student.FullName} is already permitted to borrow."
                : $"{student.FullName} is already restricted from borrowing.";
        }

        if (allowBorrowing)
        {
            student.Permit();
        }
        else
        {
            student.Restrict();
        }

        await _studentRepository.UpdateAsync(student, cancellationToken);

        return allowBorrowing
            ? $"{student.FullName} is now permitted to borrow."
            : $"{student.FullName} is now restricted from borrowing. Existing active borrowings remain intact.";
    }
}