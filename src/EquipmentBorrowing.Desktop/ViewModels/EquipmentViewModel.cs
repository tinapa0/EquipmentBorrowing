using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentViewModel : ObservableObject
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly BorrowEquipmentService _borrowService;
    private readonly BorrowingsViewModel _borrowingsViewModel;

    [ObservableProperty] private ObservableCollection<Equipment> _equipmentList = new();
    [ObservableProperty] private ObservableCollection<Student> _studentList = new();
    [ObservableProperty] private Equipment? _selectedEquipment;
    [ObservableProperty] private int _selectedStudentId;
    [ObservableProperty] private Student? _selectedStudent;
    [ObservableProperty] private DateTimeOffset? _expectedReturnDate = DateTimeOffset.Now.AddDays(7);
    [ObservableProperty] private string? _statusMessage;

    partial void OnSelectedStudentIdChanged(int value)
    {
        _selectedStudent = StudentList.FirstOrDefault(s => s.Id == value);
    }

    public EquipmentViewModel(
        IEquipmentRepository equipmentRepository,
        IStudentRepository studentRepository,
        BorrowEquipmentService borrowService,
        BorrowingsViewModel borrowingsViewModel)
    {
        _equipmentRepository = equipmentRepository;
        _studentRepository = studentRepository;
        _borrowService = borrowService;
        _borrowingsViewModel = borrowingsViewModel;

        // Connect ViewModels so returns update this view in real time
        _borrowingsViewModel.SetEquipmentViewModel(this);

        // Safely initialize data on startup
        _ = InitializeDataAsync();
    }

    private async Task InitializeDataAsync()
    {
        try
        {
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Startup Error: {ex.InnerException?.Message ?? ex.Message}";
        }
    }

    public async Task LoadDataAsync()
    {
        var items = await _equipmentRepository.GetAllAsync();
        EquipmentList = new ObservableCollection<Equipment>(items);

        var students = await _studentRepository.GetAllAsync();
        StudentList = new ObservableCollection<Student>(students);
    }

    [RelayCommand]
    private async Task BorrowAsync()
    {
        if (SelectedStudent == null)
        {
            StatusMessage = "Validation Error: Please select a student.";
            return;
        }

        if (SelectedEquipment == null)
        {
            StatusMessage = "Validation Error: Please select an equipment item.";
            return;
        }

        try
        {
            int durationDays = ExpectedReturnDate.HasValue
                ? (int)Math.Ceiling((ExpectedReturnDate.Value.DateTime - DateTime.UtcNow).TotalDays)
                : 7;

            if (durationDays <= 0)
            {
                StatusMessage = "Validation Error: Expected return date must be in the future.";
                return;
            }

            await _borrowService.ExecuteAsync(SelectedStudent.Id, SelectedEquipment.Id, ExpectedReturnDate?.DateTime);
            StatusMessage = $"Successfully borrowed {SelectedEquipment.Name}!";

            // Reload Active Borrowings list with populated Student/Equipment navigation data
            await _borrowingsViewModel.LoadBorrowingsAsync();

            // Refresh equipment list status tags
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            var actualError = ex.InnerException?.Message ?? ex.Message;
            StatusMessage = $"Borrow Failed: {actualError}";
        }
    }
}