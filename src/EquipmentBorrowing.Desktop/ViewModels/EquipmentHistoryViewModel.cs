using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentHistoryViewModel : ObservableObject
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IBorrowingRepository _borrowingRepository;

    [ObservableProperty] private ObservableCollection<Equipment> _equipmentList = new();
    [ObservableProperty] private Equipment? _selectedEquipment;
    [ObservableProperty] private ObservableCollection<Borrowing> _borrowingHistory = new();
    [ObservableProperty] private string _statusMessage = string.Empty;

    public EquipmentHistoryViewModel(IEquipmentRepository equipmentRepository, IBorrowingRepository borrowingRepository)
    {
        _equipmentRepository = equipmentRepository;
        _borrowingRepository = borrowingRepository;
    }

    public async Task LoadDataAsync()
    {
        var items = await _equipmentRepository.GetAllAsync();
        EquipmentList = new ObservableCollection<Equipment>(items);
    }

    partial void OnSelectedEquipmentChanged(Equipment? value)
    {
        if (value != null)
        {
            _ = LoadHistoryForEquipmentAsync(value.Id);
        }
        else
        {
            BorrowingHistory.Clear();
        }
    }

    private async Task LoadHistoryForEquipmentAsync(int equipmentId)
    {
        try
        {
            var history = await _borrowingRepository.GetByEquipmentIdAsync(equipmentId);
            BorrowingHistory = new ObservableCollection<Borrowing>(history);
            StatusMessage = BorrowingHistory.Any() ? string.Empty : "No borrowing history found for this equipment item.";
        }
        catch (System.Exception ex)
        {
            StatusMessage = "Error loading equipment history: " + ex.Message;
        }
    }
}