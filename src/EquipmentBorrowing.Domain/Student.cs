namespace EquipmentBorrowing.Domain;

public class Student
{
    public int Id { get; }
    public string Name { get; }
    public bool IsAllowedToBorrow { get; set; }
    public int MaxAllowedBorrowings { get; set; }

    public Student(int id, string name, bool isAllowedToBorrow = true, int maxAllowedBorrowings = 3)
    {
        Id = id;
        Name = name;
        IsAllowedToBorrow = isAllowedToBorrow;
        MaxAllowedBorrowings = maxAllowedBorrowings;
    }
}