using System;

namespace EquipmentBorrowing.Domain;

public class Equipment
{
    public int Id { get; private set; }
    public string EquipmentIdentifier { get; private set; } = string.Empty; // BR-05: Required, unique physical identifier
    public string Name { get; private set; } = string.Empty;                 // BR-06: Required name
    public string? Description { get; private set; }                        // BR-06: Optional description
    public bool IsAvailable { get; private set; } = true;                   // BR-06: Defaults to available

    protected Equipment() { } // Required for EF Core

    public Equipment(int id, string equipmentIdentifier, string name, string? description = null, bool isAvailable = true)
    {
        Id = id;
        SetEquipmentIdentifier(equipmentIdentifier);
        SetName(name);
        SetDescription(description);
        IsAvailable = isAvailable;
    }

    public void SetEquipmentIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Equipment identifier cannot be blank.", nameof(identifier));
        EquipmentIdentifier = identifier.Trim(); // BR-05: Trim surrounding whitespace
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Equipment name cannot be blank.", nameof(name));
        Name = name.Trim();
    }

    public void SetDescription(string? description)
    {
        // BR-06: Blank optional description is stored as absent (null)
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public void MarkAsBorrowed() => IsAvailable = false;
    public void MarkAsReturned() => IsAvailable = true;
}