using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class StudentHistoryViewModel : ObservableObject
{
    private readonly IStudentRepository _studentRepository;
    private readonly IBorrowingRepository _borrowingRepository;

    [ObservableProperty] private ObservableCollection<Student> _students = new();
    [ObservableProperty] private Student? _selectedStudent;
    [ObservableProperty] private ObservableCollection<Borrowing> _borrowingHistory = new();
    [ObservableProperty] private string _statusMessage = string.Empty;

    public StudentHistoryViewModel(IStudentRepository studentRepository, IBorrowingRepository borrowingRepository)
    {
        _studentRepository = studentRepository;
        _borrowingRepository = borrowingRepository;
    }

    public async Task LoadDataAsync()
    {
        var studentList = await _studentRepository.GetAllAsync();
        Students = new ObservableCollection<Student>(studentList);
    }

    partial void OnSelectedStudentChanged(Student? value)
    {
        if (value != null)
        {
            _ = LoadHistoryForStudentAsync(value.Id);
        }
        else
        {
            BorrowingHistory.Clear();
        }
    }

    private async Task LoadHistoryForStudentAsync(int studentId)
    {
        try
        {
            var history = await _borrowingRepository.GetByStudentIdAsync(studentId);
            BorrowingHistory = new ObservableCollection<Borrowing>(history);
            StatusMessage = BorrowingHistory.Any() ? string.Empty : "No borrowing history found for this student.";
        }
        catch (System.Exception ex)
        {
            StatusMessage = "Error loading student history: " + ex.Message;
        }
    }
}