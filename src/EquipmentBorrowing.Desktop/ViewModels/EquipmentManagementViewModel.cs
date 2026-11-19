using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentManagementViewModel : ObservableObject
{
    private readonly IEquipmentRepository _equipmentRepository;

    [ObservableProperty] private ObservableCollection<Equipment> _equipmentList = new();
    [ObservableProperty] private Equipment? _selectedEquipment;

    // Input fields for registration / editing
    [ObservableProperty] private string _identifier = string.Empty;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string? _description;
    [ObservableProperty] private bool _isAvailable = true;

    [ObservableProperty] private string _statusMessage = string.Empty;

    public EquipmentManagementViewModel(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task LoadEquipmentAsync()
    {
        var items = await _equipmentRepository.GetAllAsync();
        EquipmentList = new ObservableCollection<Equipment>(items);
    }

    [RelayCommand]
    private async Task RegisterEquipmentAsync()
    {
        if (string.IsNullOrWhiteSpace(Identifier) || string.IsNullOrWhiteSpace(Name))
        {
            StatusMessage = "Error: Identifier and Name cannot be empty.";
            return;
        }

        try
        {
            var newEquipment = new Equipment(0, Identifier, Name, Description, IsAvailable);
            await _equipmentRepository.AddAsync(newEquipment);

            StatusMessage = "Equipment registered successfully!";
            Identifier = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
            IsAvailable = true;

            await LoadEquipmentAsync();
        }
        catch (System.Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }
}