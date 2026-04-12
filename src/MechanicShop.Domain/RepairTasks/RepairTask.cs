using MechanicShop.Domain.Common;
using MechanicShop.Domain.RepairTasks.Parts;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.RepairTasks;

public sealed class RepairTask : AuditableEntity
{
    public string? Name { get; private set; }
    public decimal LaborCost { get; private set; }
    public int RepairDurationInMinutes { get; private set; }
    private readonly List<Part> _parts = [];
    public IEnumerable<Part> Parts => _parts.AsReadOnly();
    public decimal TotalCost => LaborCost + Parts.Sum(p => p.Cost * p.Quantity);

    private RepairTask()
    {
    }

    private RepairTask(Guid id, string name, decimal laborCost, int repairDurationInMinutes, List<Part> parts) : base(id)
    {
        Name = name;
        LaborCost = laborCost;
        RepairDurationInMinutes = repairDurationInMinutes;
        _parts = parts;
    }

    private static Result<bool> Validate(string name, decimal laborCost, int repairDurationInMinutes)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return RepairTaskErrors.NameRequired;
        }

        if (laborCost < 1 || laborCost > 10_000)
        {
            return RepairTaskErrors.LaborCostInvalid;
        }

        if (repairDurationInMinutes < 5)
        {
            return RepairTaskErrors.DurationInvalid;
        }

        return true;
    }

    public static Result<RepairTask> Create(Guid id, string name, decimal laborCost, int repairDurationInMinutes, List<Part> parts)
    {
        var validationResult = Validate(name, laborCost, repairDurationInMinutes);
        if (validationResult.IsError)
        {
            return validationResult.Errors;
        }
        return new RepairTask(id, name, laborCost, repairDurationInMinutes, parts);
    }

    public Result<Updated> Update(string name, decimal laborCost, int repairDurationInMinutes)
    {
        var validationResult = Validate(name, laborCost, repairDurationInMinutes);
        if (validationResult.IsError)
        {
            return validationResult.Errors;
        }
        Name = name;
        LaborCost = laborCost;
        RepairDurationInMinutes = repairDurationInMinutes;
        return Result.Updated;
    }

    public Result<Updated> SynchronizeVehicles(List<Part> incomingParts)
    {
        _parts.RemoveAll(existing => incomingParts.All(p => p.Id != existing.Id));

        foreach (var incoming in incomingParts)
        {
            var existing = _parts.FirstOrDefault(p => p.Id == incoming.Id);
            if (existing is null)
            {
                _parts.Add(incoming);
            }
            else
            {
                var updatedResult = existing.Update(incoming.Name!, incoming.Cost, incoming.Quantity);
                if (updatedResult.IsError)
                {
                    return updatedResult.Errors;
                }
            }
        }

        return Result.Updated;
    }
}
