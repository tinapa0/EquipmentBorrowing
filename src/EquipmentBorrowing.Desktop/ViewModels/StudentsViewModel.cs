using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class StudentsViewModel : ObservableObject
{
    private readonly StudentManagementService _studentService;

    [ObservableProperty] private ObservableCollection<Student> _students = new();
    [ObservableProperty] private Student? _selectedStudent;

    // Form fields (used for registering a new student or editing the selected student)
    [ObservableProperty] private string _fullName = string.Empty;
    [ObservableProperty] private string _studentNumber = string.Empty;
    [ObservableProperty] private string _contactInfo = string.Empty;
    [ObservableProperty] private bool _isAllowedToBorrow = true;

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public StudentsViewModel(StudentManagementService studentService)
    {
        _studentService = studentService;
    }

    partial void OnSelectedStudentChanged(Student? value)
    {
        if (value is not null)
        {
            FullName = value.FullName;
            StudentNumber = value.StudentNumber;
            ContactInfo = value.ContactInfo ?? string.Empty;
            IsAllowedToBorrow = value.IsAllowedToBorrow;
        }
    }

    public async Task LoadStudentsAsync()
    {
        IsBusy = true;
        try
        {
            var studentsList = await _studentService.GetAllStudentsAsync();
            Students = new ObservableCollection<Student>(studentsList);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading students: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ClearForm()
    {
        SelectedStudent = null;
        FullName = string.Empty;
        StudentNumber = string.Empty;
        ContactInfo = string.Empty;
        IsAllowedToBorrow = true;
        StatusMessage = string.Empty;
    }

    // UC-12: Register Student
    [RelayCommand]
    private async Task RegisterStudentAsync()
    {
        IsBusy = true;
        try
        {
            var created = await _studentService.RegisterStudentAsync(
                StudentNumber,
                FullName,
                ContactInfo);

            StatusMessage = $"Student '{created.FullName}' ({created.StudentNumber}) registered.";

            SelectedStudent = null;
            FullName = string.Empty;
            StudentNumber = string.Empty;
            ContactInfo = string.Empty;
            IsAllowedToBorrow = true;

            await LoadStudentsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    // UC-13: Update Student Information (preserves Id and borrowing history)
    [RelayCommand]
    private async Task UpdateStudentAsync()
    {
        if (SelectedStudent is null)
        {
            StatusMessage = "Error: Select a student from the list to update.";
            return;
        }

        IsBusy = true;
        try
        {
            var targetId = SelectedStudent.Id;
            var updated = await _studentService.UpdateStudentInfoAsync(
                targetId,
                StudentNumber,
                FullName,
                ContactInfo);

            StatusMessage = $"Updated '{updated.FullName}' ({updated.StudentNumber}). Existing borrowing history preserved.";

            await LoadStudentsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    // UC-14: Permit Borrowing
    [RelayCommand]
    private async Task PermitBorrowingAsync(Student? student)
    {
        var target = student ?? SelectedStudent;
        if (target is null)
        {
            StatusMessage = "Error: Select a student first.";
            return;
        }

        IsBusy = true;
        try
        {
            StatusMessage = await _studentService.SetEligibilityAsync(target.Id, allowBorrowing: true);
            await LoadStudentsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    // UC-14: Restrict Borrowing
    [RelayCommand]
    private async Task RestrictBorrowingAsync(Student? student)
    {
        var target = student ?? SelectedStudent;
        if (target is null)
        {
            StatusMessage = "Error: Select a student first.";
            return;
        }

        IsBusy = true;
        try
        {
            StatusMessage = await _studentService.SetEligibilityAsync(target.Id, allowBorrowing: false);
            await LoadStudentsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}