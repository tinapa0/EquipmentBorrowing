using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Repositories;
using System;
using System.Collections.ObjectModel;

using System.Threading.Tasks;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentViewModel : ObservableObject
{
    private readonly BorrowEquipmentService _borrowService;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IStudentRepository _studentRepository;

    [ObservableProperty] private ObservableCollection<Equipment> _equipmentList = new();
    [ObservableProperty] private ObservableCollection<Student> _studentList = new();
    [ObservableProperty] private Equipment? _selectedEquipment;
    [ObservableProperty] private Student? _selectedStudent;
    [ObservableProperty] private DateTimeOffset? _expectedReturnDate = DateTimeOffset.Now.AddDays(7);
    [ObservableProperty] private string? _statusMessage;

    public EquipmentViewModel(
        BorrowEquipmentService borrowService,
        IEquipmentRepository equipmentRepository,
        IStudentRepository studentRepository)
    {
        _borrowService = borrowService;
        _equipmentRepository = equipmentRepository;
        _studentRepository = studentRepository;
        _ = LoadDataAsync();
    }

    public async Task LoadDataAsync()
    {
        EquipmentList.Clear();
        var items = await _equipmentRepository.GetAllAsync();
        foreach (var item in items) EquipmentList.Add(item);

        StudentList.Clear();
        var students = await _studentRepository.GetAllAsync();
        foreach (var student in students) StudentList.Add(student);
    }

    [RelayCommand]
    private async Task BorrowAsync()
    {
        if (SelectedStudent == null) { StatusMessage = "Validation Error: Please select a student."; return; }
        if (SelectedEquipment == null) { StatusMessage = "Validation Error: Please select an equipment item."; return; }

        try
        {
            // Calculate days difference (defaulting to 7 days if ExpectedReturnDate is null)
            int durationDays = ExpectedReturnDate.HasValue
                ? (int)Math.Ceiling((ExpectedReturnDate.Value.DateTime - DateTime.UtcNow).TotalDays)
                : 7;

            // Ensure duration is at least 1 day
            if (durationDays <= 0)
            {
                StatusMessage = "Validation Error: Expected return date must be in the future.";
                return;
            }

            // Correct - passing DateTime directly:
            await _borrowService.ExecuteAsync(SelectedStudent.Id, SelectedEquipment.Id, ExpectedReturnDate?.DateTime);
            StatusMessage = $"Successfully borrowed {SelectedEquipment.Name}!";
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }
}