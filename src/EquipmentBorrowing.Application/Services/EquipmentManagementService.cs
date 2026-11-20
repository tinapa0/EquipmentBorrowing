using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class EquipmentManagementService
{
    private readonly IEquipmentRepository _equipmentRepository;

    public EquipmentManagementService(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<IReadOnlyList<Equipment>> GetAllEquipmentAsync(CancellationToken cancellationToken = default)
    {
        var items = await _equipmentRepository.GetAllAsync(cancellationToken);
        return items.OrderBy(e => e.Id).ToList();
    }

    // UC-15: Register Equipment (BR-05, BR-06, BR-10)
    public async Task<Equipment> RegisterEquipmentAsync(
        string identifier,
        string name,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Equipment identifier is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Equipment name is required.");

        var normalizedId = identifier.Trim();
        var existingItems = await _equipmentRepository.GetAllAsync(cancellationToken);

        if (existingItems.Any(e => string.Equals(e.EquipmentIdentifier.Trim(), normalizedId, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Equipment with identifier '{normalizedId}' already exists.");

        // BR-06: Newly registered equipment is available and blank description becomes null
        var equipment = new Equipment(
            id: 0,
            equipmentIdentifier: normalizedId,
            name: name.Trim(),
            description: description,
            isAvailable: true);

        await _equipmentRepository.AddAsync(equipment, cancellationToken);
        return equipment;
    }

    // UC-16: Update Equipment Information (BR-05, BR-06, BR-07, BR-14)
    public async Task<Equipment> UpdateEquipmentInfoAsync(
        int equipmentId,
        string identifier,
        string name,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Equipment identifier is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Equipment name is required.");

        var equipment = await _equipmentRepository.GetByIdAsync(equipmentId, cancellationToken)
            ?? throw new InvalidOperationException("Selected equipment item was not found.");

        var normalizedId = identifier.Trim();
        var existingItems = await _equipmentRepository.GetAllAsync(cancellationToken);

        if (existingItems.Any(e =>
            e.Id != equipmentId &&
            string.Equals(e.EquipmentIdentifier.Trim(), normalizedId, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Another item already uses identifier '{normalizedId}'.");
        }

        // Preserves Id and IsAvailable; keeps existing borrowing history intact
        equipment.SetEquipmentIdentifier(normalizedId);
        equipment.SetName(name.Trim());
        equipment.SetDescription(description);

        await _equipmentRepository.UpdateAsync(equipment, cancellationToken);
        return equipment;
    }
}