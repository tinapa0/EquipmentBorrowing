using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class BorrowingsViewModel : ObservableObject
{
    private readonly ReturnEquipmentService _returnService;
    private readonly IBorrowingRepository _borrowingRepository;

    [ObservableProperty] private ObservableCollection<Borrowing> _activeBorrowings = new();
    [ObservableProperty] private Borrowing? _selectedBorrowing;
    [ObservableProperty] private string? _statusMessage;

    public BorrowingsViewModel(ReturnEquipmentService returnService, IBorrowingRepository borrowingRepository)
    {
        _returnService = returnService;
        _borrowingRepository = borrowingRepository;
        _ = LoadBorrowingsAsync();
    }

    public async Task LoadBorrowingsAsync()
    {
        ActiveBorrowings.Clear();
        var items = await _borrowingRepository.GetActiveBorrowingsAsync();
        foreach (var item in items) ActiveBorrowings.Add(item);
    }

    [RelayCommand]
    private async Task ReturnAsync()
    {
        if (SelectedBorrowing == null) { StatusMessage = "Validation Error: Please select an active borrowing record."; return; }

        try
        {
            await _returnService.ReturnAsync(SelectedBorrowing.Id);
            StatusMessage = "Equipment successfully returned!";
            await LoadBorrowingsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Return Failed: {ex.Message}";
        }
    }
}