using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.RepairTasks;

public static class RepairTaskErrors
{
    public static Error NameRequired => Error.Validation("RepairTask_Name_Required", "Name is required.");
    public static Error LaborCostInvalid => Error.Validation("RepairTask_LaborCost_Invalid", "Labor cost must be between 1 and 10,000.");
    public static Error DurationInvalid => Error.Validation("RepairTask_Duration_Invalid", "Duration must be more than 5 minutes");
    public static Error PartsRequired => Error.Validation("RepairTask_Parts_Required", "At least one part is required.");
    public static Error PartNameRequired => Error.Validation("RepairTask_Parts_Name_Required", "All parts must have a name.");
    public static Error AtLeastOneRepairTaskIsRequired => Error.Validation("RepairTask_Required", "At least one repair task must be specified.");
    public static Error InUse => Error.Conflict("RepairTask_InUse", "Cannot delete a repair task that is used in work orders.");
    public static Error DuplicateName => Error.Conflict("RepairTaskPart_Duplicate", "A part with the same name already exists in this repair task.");
}
