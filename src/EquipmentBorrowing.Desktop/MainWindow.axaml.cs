using Avalonia.Controls;
using Avalonia.Interactivity;
using EquipmentBorrowing.Desktop.Views;
using EquipmentBorrowing.Desktop.ViewModels;

namespace EquipmentBorrowing.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // wire up navigation
        var navEquipment = this.FindControl<Button>("NavEquipment");
        if (navEquipment != null)
        {
            navEquipment.Click += NavEquipment_Click;
        }
    }

    private void NavEquipment_Click(object? sender, RoutedEventArgs e)
    {
        var vm = new EquipmentViewModel();
        var view = new EquipmentView { DataContext = vm };
        var content = this.FindControl<ContentControl>("MainContent");
        if (content != null)
        {
            content.Content = view;
        }
    }
}
