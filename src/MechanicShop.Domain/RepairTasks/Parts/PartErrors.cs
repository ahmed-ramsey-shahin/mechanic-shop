using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.RepairTasks.Parts;

public static class PartErrors
{
    public static readonly Error NameRequired = Error.Validation("Part_Name_Required", "Part name is required.");
    public static readonly Error CostInvalid = Error.Validation("Part_Cost_Invalid", "Part cost must be between 1 and 100,000.");
    public static readonly Error QuantityInvalid = Error.Validation("Part_Quantity_Invalid", "Quantity must be between 1 and 50.");
}
