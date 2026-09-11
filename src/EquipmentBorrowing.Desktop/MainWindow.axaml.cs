using Avalonia.Controls;
using Avalonia.Interactivity;
using EquipmentBorrowing.Desktop.Views;
using EquipmentBorrowing.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

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

        var navBorrowings = this.FindControl<Button>("NavBorrowings");
        if (navBorrowings != null)
        {
            navBorrowings.Click += NavBorrowings_Click;
        }
    }

    private void HookStatusUpdates(INotifyPropertyChanged vm)
    {
        var statusText = this.FindControl<TextBlock>("StatusText");
        if (statusText == null)
            return;

        vm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "Status")
            {
                var prop = s.GetType().GetProperty("Status");
                var val = prop?.GetValue(s) as string ?? string.Empty;
                statusText.Text = val;
            }
            else if (e.PropertyName == "StatusMessage")
            {
                var prop = s.GetType().GetProperty("StatusMessage");
                var val = prop?.GetValue(s) as string ?? string.Empty;
                statusText.Text = val;
            }
        };
    }

    private void NavEquipment_Click(object? sender, RoutedEventArgs e)
    {
        // try to resolve via DI if available, otherwise new
        EquipmentViewModel vm = App.Services?.GetService<EquipmentViewModel>() ?? new EquipmentViewModel();
        var view = new EquipmentView { DataContext = vm };
        HookStatusUpdates(vm);

        var content = this.FindControl<ContentControl>("MainContent");
        if (content != null)
        {
            content.Content = view;
        }
    }

    private void NavBorrowings_Click(object? sender, RoutedEventArgs e)
    {
        ActiveBorrowingsViewModel vm = App.Services?.GetService<ActiveBorrowingsViewModel>() ?? new ActiveBorrowingsViewModel();
        var view = new ActiveBorrowingsView { DataContext = vm };
        HookStatusUpdates(vm);

        var content = this.FindControl<ContentControl>("MainContent");
        if (content != null)
        {
            content.Content = view;
        }
    }
}
