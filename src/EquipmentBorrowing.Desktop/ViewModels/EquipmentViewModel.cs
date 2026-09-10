using System.Collections.ObjectModel;
using System.ComponentModel;

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

    public EquipmentViewModel()
    {
        // Sample data for initial UI; later this should be replaced with real data from Application layer
        Equipment.Add(new EquipmentItem { Id = "EQ-001", Name = "Projector", Description = "LCD Projector" });
        Equipment.Add(new EquipmentItem { Id = "EQ-002", Name = "Laptop", Description = "Dell Latitude" });
        Equipment.Add(new EquipmentItem { Id = "EQ-003", Name = "Microphone", Description = "Wireless mic" });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
