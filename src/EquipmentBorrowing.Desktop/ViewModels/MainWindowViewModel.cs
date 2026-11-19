using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private ObservableObject _currentView;

    public EquipmentViewModel EquipmentVm { get; }
    public BorrowingsViewModel BorrowingsVm { get; }
    public StudentsViewModel StudentsVm { get; } // Add this property
    public EquipmentManagementViewModel EquipmentManagementVm { get; }

    public CheckoutViewModel CheckoutVm { get; }
    public StudentHistoryViewModel StudentHistoryVm { get; }
    public EquipmentHistoryViewModel EquipmentHistoryVm { get; }
    public BorrowingSearchViewModel BorrowingSearchVm { get; }

    public MainWindowViewModel(
        EquipmentViewModel equipmentVm,
        BorrowingsViewModel borrowingsVm,
        StudentsViewModel studentsVm, // Inject StudentsViewModel here
        EquipmentManagementViewModel equipmentManagementVm,
        CheckoutViewModel checkoutVm,
        StudentHistoryViewModel studentHistoryVm,
        EquipmentHistoryViewModel equipmentHistoryVm,
        BorrowingSearchViewModel borrowingSearchVm)
    {
        EquipmentVm = equipmentVm;
        BorrowingsVm = borrowingsVm;
        StudentsVm = studentsVm;
        EquipmentManagementVm = equipmentManagementVm;
        CheckoutVm = checkoutVm;
        StudentHistoryVm = studentHistoryVm;
        EquipmentHistoryVm = equipmentHistoryVm;
        BorrowingSearchVm = borrowingSearchVm;
        _currentView = EquipmentVm;
        
    }

    [RelayCommand]
    private void NavigateToEquipment() => CurrentView = EquipmentVm;

    [RelayCommand]
    private async Task NavigateToBorrowings()
    {
        CurrentView = BorrowingsVm;
        await BorrowingsVm.LoadBorrowingsAsync();
    }

    [RelayCommand]
    private async Task NavigateToStudents() // Add navigation command for students
    {
        CurrentView = StudentsVm;
        await StudentsVm.LoadStudentsAsync(); // We will make sure this method exists in StudentsViewModel
    }

    [RelayCommand]
    private async Task NavigateToEquipmentManagement() // 3. Add navigation command
    {
        CurrentView = EquipmentManagementVm;
        await EquipmentManagementVm.LoadEquipmentAsync();
    }

    [RelayCommand]
    private async Task NavigateToCheckout()
    {
        CurrentView = CheckoutVm;
        await CheckoutVm.LoadCheckoutDataAsync();
    }

    [RelayCommand]
    private async Task NavigateToStudentHistory()
    {
        CurrentView = StudentHistoryVm;
        await StudentHistoryVm.LoadDataAsync();
    }

    [RelayCommand]
    private async Task NavigateToEquipmentHistory()
    {
        CurrentView = EquipmentHistoryVm;
        await EquipmentHistoryVm.LoadDataAsync();
    }
   
    [RelayCommand]
    private async Task NavigateToBorrowingSearch()
    {
        CurrentView = BorrowingSearchVm;
        await BorrowingSearchVm.LoadDataAsync();
    }
}