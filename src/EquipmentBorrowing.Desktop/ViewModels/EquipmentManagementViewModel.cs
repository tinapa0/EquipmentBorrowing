using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentManagementViewModel : ObservableObject
{
    private readonly EquipmentManagementService _equipmentService;

    [ObservableProperty] private ObservableCollection<Equipment> _equipmentList = new();
    [ObservableProperty] private Equipment? _selectedEquipment;

    // Form fields for registration / editing
    [ObservableProperty] private string _identifier = string.Empty;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private bool _selectedIsAvailable = true;

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public EquipmentManagementViewModel(EquipmentManagementService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    partial void OnSelectedEquipmentChanged(Equipment? value)
    {
        if (value is not null)
        {
            Identifier = value.EquipmentIdentifier;
            Name = value.Name;
            Description = value.Description ?? string.Empty;
            SelectedIsAvailable = value.IsAvailable;
        }
    }

    public async Task LoadEquipmentAsync()
    {
        IsBusy = true;
        try
        {
            var items = await _equipmentService.GetAllEquipmentAsync();
            EquipmentList = new ObservableCollection<Equipment>(items);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading equipment: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ClearForm()
    {
        SelectedEquipment = null;
        Identifier = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
        SelectedIsAvailable = true;
        StatusMessage = string.Empty;
    }

    // UC-15: Register Equipment
    [RelayCommand]
    private async Task RegisterEquipmentAsync()
    {
        IsBusy = true;
        try
        {
            var created = await _equipmentService.RegisterEquipmentAsync(
                Identifier,
                Name,
                Description);

            StatusMessage = $"Registered '{created.EquipmentIdentifier} - {created.Name}' (Available).";

            SelectedEquipment = null;
            Identifier = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
            SelectedIsAvailable = true;

            await LoadEquipmentAsync();
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

    // UC-16: Update Equipment Information (preserves Id, availability, and borrowing history)
    [RelayCommand]
    private async Task UpdateEquipmentAsync()
    {
        if (SelectedEquipment is null)
        {
            StatusMessage = "Error: Select an equipment item from the list to update.";
            return;
        }

        IsBusy = true;
        try
        {
            var targetId = SelectedEquipment.Id;
            var updated = await _equipmentService.UpdateEquipmentInfoAsync(
                targetId,
                Identifier,
                Name,
                Description);

            StatusMessage = $"Updated '{updated.EquipmentIdentifier} - {updated.Name}'. History and availability preserved.";

            await LoadEquipmentAsync();
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