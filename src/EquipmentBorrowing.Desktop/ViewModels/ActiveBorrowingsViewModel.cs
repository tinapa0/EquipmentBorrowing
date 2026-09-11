using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Desktop.ViewModels;

public class ActiveBorrowingItem
{
    public int BorrowingId { get; set; }
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ActiveBorrowingsViewModel : INotifyPropertyChanged
{
    public ObservableCollection<ActiveBorrowingItem> ActiveBorrowings { get; } = new ObservableCollection<ActiveBorrowingItem>();

    private ActiveBorrowingItem? _selected;
    public ActiveBorrowingItem? Selected
    {
        get => _selected;
        set
        {
            if (_selected != value)
            {
                _selected = value;
                OnPropertyChanged();
                ((RelayCommand)ReturnCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public ICommand ReturnCommand { get; }
    public ICommand RefreshCommand { get; }

    private readonly EquipmentBorrowing.Infrastructure.Repositories.InMemoryStudentRepository _studentRepo;
    private readonly EquipmentBorrowing.Infrastructure.Repositories.InMemoryEquipmentRepository _equipmentRepo;
    private readonly EquipmentBorrowing.Infrastructure.Repositories.InMemoryBorrowingRepository _borrowingRepo;

    public string StatusMessage { get; private set; } = string.Empty;

    public ActiveBorrowingsViewModel()
    {
        _studentRepo = new EquipmentBorrowing.Infrastructure.Repositories.InMemoryStudentRepository();
        _equipmentRepo = new EquipmentBorrowing.Infrastructure.Repositories.InMemoryEquipmentRepository();
        _borrowingRepo = new EquipmentBorrowing.Infrastructure.Repositories.InMemoryBorrowingRepository();

        ReturnCommand = new RelayCommand(async _ => await ReturnAsync(), _ => Selected != null);
        RefreshCommand = new RelayCommand(async _ => await LoadAsync());

        // create a demo borrowing in the in-memory repository
        var demo = new Borrowing(9001, 1, 101, DateTime.UtcNow.AddDays(7));
        _borrowingRepo.AddAsync(demo).GetAwaiter().GetResult();

        LoadAsync().GetAwaiter().GetResult();
    }

    public async Task LoadAsync()
    {
        ActiveBorrowings.Clear();

        // for demo, try to load the demo borrowing
        var b = await _borrowingRepo.GetByIdAsync(9001);
        if (b != null && b.Status == BorrowingStatus.Active)
        {
            var equipment = await _equipmentRepo.GetByIdAsync(b.EquipmentId);
            ActiveBorrowings.Add(new ActiveBorrowingItem
            {
                BorrowingId = b.Id,
                EquipmentId = b.EquipmentId,
                EquipmentName = equipment?.Name ?? string.Empty,
                StudentId = b.StudentId,
                ExpectedReturnDate = b.ExpectedReturnDate,
                Status = b.Status.ToString()
            });
        }
    }

    private async Task ReturnAsync()
    {
        if (Selected == null)
            return;

        try
        {
            var service = new ReturnEquipmentService(_equipmentRepo, _borrowingRepo);
            var returned = await service.ExecuteAsync(Selected.BorrowingId);

            // remove from list
            ActiveBorrowings.Remove(Selected);
            Selected = null;

            StatusMessage = $"Returned borrowing {returned.Id}";
            OnPropertyChanged(nameof(StatusMessage));
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            OnPropertyChanged(nameof(StatusMessage));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private class RelayCommand : ICommand
    {
        private readonly Func<object?, Task> _executeAsync;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Func<object?, Task> executeAsync, Predicate<object?>? canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public async void Execute(object? parameter) => await _executeAsync(parameter);

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
