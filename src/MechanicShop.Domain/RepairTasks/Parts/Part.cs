using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.RepairTasks.Parts;

public sealed class Part : AuditableEntity
{
    public string? Name { get; private set; }
    public decimal Cost { get; private set; }
    public int Quantity { get; private set; }

    private Part()
    {
    }

    private Part(Guid id, string name, decimal cost, int quantity) : base(id)
    {
        Name = name;
        Cost = cost;
        Quantity = quantity;
    }

    private static Result<bool> Validate(string name, decimal cost, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return PartErrors.NameRequired;
        }

        if (cost < 1 || cost > 10_000)
        {
            return PartErrors.CostInvalid;
        }

        if (quantity < 1 || quantity > 50)
        {
            return PartErrors.QuantityInvalid;
        }

        return true;
    }

    public static Result<Part> Create(Guid id, string name, decimal cost, int quantity)
    {
        var validationResult = Validate(name, cost, quantity);
        if (validationResult.IsError)
        {
            return validationResult.Errors;
        }
        return new Part(id, name, cost, quantity);
    }

    public Result<Updated> Update(string name, decimal cost, int quantity)
    {
        var validationResult = Validate(name, cost, quantity);
        if (validationResult.IsError)
        {
            return validationResult.Errors;
        }
        Name = name;
        Cost = cost;
        Quantity = quantity;
        return Result.Updated;
    }
}
