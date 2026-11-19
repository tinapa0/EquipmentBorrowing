using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class BorrowingSearchViewModel : ObservableObject
{
    private readonly IBorrowingRepository _borrowingRepository;
    private List<Borrowing> _allBorrowings = new();

    [ObservableProperty] private ObservableCollection<Borrowing> _filteredBorrowings = new();

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private string _selectedStatusFilter = "All"; // All, Active, Returned
    [ObservableProperty] private string _selectedSortField = "Borrow Date"; // Borrow Date, Expected Return
    [ObservableProperty] private bool _isAscending = true;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public List<string> StatusOptions { get; } = new() { "All", "Active", "Returned" };
    public List<string> SortOptions { get; } = new() { "Borrow Date", "Expected Return" };

    public BorrowingSearchViewModel(IBorrowingRepository borrowingRepository)
    {
        _borrowingRepository = borrowingRepository;
    }

    public async Task LoadDataAsync()
    {
        try
        {
            var data = await _borrowingRepository.GetAllAsync();
            _allBorrowings = data.ToList();
            ApplyFilterAndSort();
        }
        catch (Exception ex)
        {
            StatusMessage = "Error loading borrowings: " + ex.Message;
        }
    }

    partial void OnSearchTextChanged(string value) => ApplyFilterAndSort();
    partial void OnSelectedStatusFilterChanged(string value) => ApplyFilterAndSort();
    partial void OnSelectedSortFieldChanged(string value) => ApplyFilterAndSort();
    partial void OnIsAscendingChanged(bool value) => ApplyFilterAndSort();

    [RelayCommand]
    private void ClearCriteria()
    {
        SearchText = string.Empty;
        SelectedStatusFilter = "All";
        SelectedSortField = "Borrow Date";
        IsAscending = true;
        StatusMessage = string.Empty;
    }

    private void ApplyFilterAndSort()
    {
        var query = _allBorrowings.AsEnumerable();

        // 1. Status Filter
        if (SelectedStatusFilter == "Active")
        {
            query = query.Where(b => b.Status == BorrowingStatus.Active);
        }
        else if (SelectedStatusFilter == "Returned")
        {
            query = query.Where(b => b.Status == BorrowingStatus.Returned);
        }

        // 2. Search Text (case-insensitive partial match on Student Name, Student Number, or Equipment Name)[cite: 8, 19]
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var term = SearchText.Trim();
            query = query.Where(b =>
                (b.Student != null && (
                    b.Student.FullName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    b.Student.StudentNumber.Contains(term, StringComparison.OrdinalIgnoreCase))) ||
                (b.Equipment != null && b.Equipment.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            );
        }

        // 3. Sorting (with stable identifier tie-breaker)[cite: 8, 19]
        query = SelectedSortField switch
        {
            "Expected Return" => IsAscending
                ? query.OrderBy(b => b.ExpectedReturnDate).ThenBy(b => b.Id)
                : query.OrderByDescending(b => b.ExpectedReturnDate).ThenBy(b => b.Id),
            _ => IsAscending
                ? query.OrderBy(b => b.BorrowedDate).ThenBy(b => b.Id)
                : query.OrderByDescending(b => b.BorrowedDate).ThenBy(b => b.Id)
        };

        var resultList = query.ToList();
        FilteredBorrowings = new ObservableCollection<Borrowing>(resultList);
        StatusMessage = FilteredBorrowings.Any() ? string.Empty : "No matching borrowing records found.";
    }
}