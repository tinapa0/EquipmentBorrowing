using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private ObservableObject _currentView;

    public EquipmentViewModel EquipmentVm { get; }
    public BorrowingsViewModel BorrowingsVm { get; }

    public MainWindowViewModel(EquipmentViewModel equipmentVm, BorrowingsViewModel borrowingsVm)
    {
        EquipmentVm = equipmentVm;
        BorrowingsVm = borrowingsVm;
        _currentView = EquipmentVm;
    }

    [RelayCommand]
    private void NavigateToEquipment() => CurrentView = EquipmentVm;

    [RelayCommand]
    private async Task NavigateToBorrowings()
    {
        await BorrowingsVm.LoadBorrowingsAsync();
        CurrentView = BorrowingsVm;
    }
}