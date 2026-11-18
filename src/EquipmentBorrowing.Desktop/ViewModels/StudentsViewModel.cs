using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Repositories;
using EquipmentBorrowing.Application.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class StudentsViewModel : ObservableObject
{
    private readonly IStudentRepository _studentRepository;

    [ObservableProperty] private ObservableCollection<Student> _students = new();
    [ObservableProperty] private Student? _selectedStudent;

    // Input fields for registering
    [ObservableProperty] private string _fullName = string.Empty;
    [ObservableProperty] private string _studentNumber = string.Empty;
    [ObservableProperty] private bool _isAllowedToBorrow = true;

    [ObservableProperty] private string _statusMessage = string.Empty;

    public StudentsViewModel(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task LoadStudentsAsync()
    {
        var studentsList = await _studentRepository.GetAllAsync();
        Students = new ObservableCollection<Student>(studentsList);
    }

    [RelayCommand]
    private async Task RegisterStudentAsync()
    {
        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(StudentNumber))
        {
            StatusMessage = "Error: Full Name and Student Number cannot be empty.";
            return;
        }

        try
        {
            // Use the domain constructor respecting private setters
            var newStudent = new Student(0, StudentNumber, FullName, null, IsAllowedToBorrow);

            await _studentRepository.AddAsync(newStudent);
            StatusMessage = "Student registered successfully!";

            // Clear inputs and reload
            FullName = string.Empty;
            StudentNumber = string.Empty;
            IsAllowedToBorrow = true;
            await LoadStudentsAsync();
        }
        catch (System.Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ToggleEligibilityAsync(Student? student)
    {
        if (student == null) return;

        if (student.IsAllowedToBorrow)
            student.Restrict();
        else
            student.Permit();

        await _studentRepository.UpdateAsync(student);
        StatusMessage = $"Updated eligibility for {student.FullName}.";
        await LoadStudentsAsync();
    }
}