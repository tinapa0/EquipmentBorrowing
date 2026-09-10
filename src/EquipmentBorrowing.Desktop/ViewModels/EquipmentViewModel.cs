using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EquipmentBorrowing.Desktop.ViewModels;

public class EquipmentItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class EquipmentViewModel : INotifyPropertyChanged
{
    public ObservableCollection<EquipmentItem> Equipment { get; } = new ObservableCollection<EquipmentItem>();

    private EquipmentItem? _selected;
    public EquipmentItem? Selected
    {
        get => _selected;
        set
        {
            if (_selected != value)
            {
                _selected = value;
                OnPropertyChanged();
                ((RelayCommand)BorrowCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public ICommand BorrowCommand { get; }
    public ICommand RefreshCommand { get; }

    public EquipmentViewModel()
    {
        BorrowCommand = new RelayCommand(async _ => await BorrowAsync(), _ => Selected != null);
        RefreshCommand = new RelayCommand(async _ => await LoadAsync());

        // initial sample data (replace with application service calls later)
        Equipment.Add(new EquipmentItem { Id = "EQ-001", Name = "Projector", Description = "LCD Projector" });
        Equipment.Add(new EquipmentItem { Id = "EQ-002", Name = "Laptop", Description = "Dell Latitude" });
        Equipment.Add(new EquipmentItem { Id = "EQ-003", Name = "Microphone", Description = "Wireless mic" });
    }

    public async Task LoadAsync()
    {
        // TODO: replace with call to application layer to load equipment
        await Task.CompletedTask;
    }

    private async Task BorrowAsync()
    {
        if (Selected == null)
            return;

        // TODO: invoke application service to borrow equipment
        await Task.CompletedTask;
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
