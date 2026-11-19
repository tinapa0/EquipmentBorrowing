using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class SelectableEquipmentItem : ObservableObject
{
    public Equipment Equipment { get; }
    [ObservableProperty] private bool _isSelected;

    public SelectableEquipmentItem(Equipment equipment)
    {
        Equipment = equipment;
    }
}

public partial class CheckoutViewModel : ObservableObject
{
    private readonly BorrowEquipmentService _borrowService;
    private readonly IStudentRepository _studentRepository;
    private readonly IEquipmentRepository _equipmentRepository;

    [ObservableProperty] private ObservableCollection<Student> _students = new();
    [ObservableProperty] private Student? _selectedStudent;

    [ObservableProperty] private ObservableCollection<SelectableEquipmentItem> _availableEquipment = new();

    [ObservableProperty] private DateTimeOffset _expectedReturnDate = DateTimeOffset.Now.AddDays(7);
    [ObservableProperty] private string _statusMessage = string.Empty;

    public CheckoutViewModel(
        BorrowEquipmentService borrowService,
        IStudentRepository studentRepository,
        IEquipmentRepository equipmentRepository)
    {
        _borrowService = borrowService;
        _studentRepository = studentRepository;
        _equipmentRepository = equipmentRepository;
    }

    public async Task LoadCheckoutDataAsync()
    {
        // Load eligible students
        var studentList = await _studentRepository.GetAllAsync();
        Students = new ObservableCollection<Student>(studentList);

        // Load only available equipment for checkout
        var equipmentList = await _equipmentRepository.GetAllAsync();
        var availableItems = equipmentList
            .Where(e => e.IsAvailable)
            .Select(e => new SelectableEquipmentItem(e));

        AvailableEquipment = new ObservableCollection<SelectableEquipmentItem>(availableItems);
        StatusMessage = string.Empty;
    }

    [RelayCommand]
    private async Task ProcessCheckoutAsync()
    {
        if (SelectedStudent == null)
        {
            StatusMessage = "Error: Please select a student.";
            return;
        }

        var selectedIds = AvailableEquipment
            .Where(item => item.IsSelected)
            .Select(item => item.Equipment.Id)
            .ToList();

        if (!selectedIds.Any())
        {
            StatusMessage = "Error: Please select at least one equipment item to check out.";
            return;
        }

        try
        {
            DateTime returnDateDateTime = ExpectedReturnDate.DateTime;
            await _borrowService.ExecuteAsync(SelectedStudent.Id, selectedIds, returnDateDateTime);

            StatusMessage = $"Successfully checked out {selectedIds.Count} item(s) for {SelectedStudent.FullName}!";

            // Refresh data list after successful checkout
            await LoadCheckoutDataAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Checkout Failed: {ex.Message}";
        }
    }
}