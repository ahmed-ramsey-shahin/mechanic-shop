using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enums;

namespace MechanicShop.Domain.WorkOrders;

public static class WorkOrderErrors
{
    public static Error VehicleIdRequired => Error.Validation("WorkOrder_Vehicle_Required", "Vehicle Id is required");
    public static Error RepairTasksRequired => Error.Validation("WorkOrder_RepairTasks_Required", "At least one repair task is required");
    public static Error LaborIdRequired => Error.Validation("WorkOrder_LaborId_Required", "Labor Id is required");
    public static Error InvalidTiming => Error.Conflict("WorkOrder_Timing_Invalid", "End time must be after start time.");
    public static Error SpotInvalid => Error.Validation("WorkOrder_Spot_Invalid", "The provided spot is invalid");
    public static Error Readonly => Error.Conflict("WorkOrder_Readonly", "WorkOrder is read-only.");
    public static Error TimingReadonly(WorkOrderState state) => Error.Conflict("WorkOrder_Timing_Readonly", $"Can't Modify timing when WorkOrder status is '{state}'.");
    public static Error InvalidStateTransition => Error.Conflict("WorkOrder_StateTransirtion_Invalid", "Invalid State transition.");
    public static Error RepairTaskAlreadyAdded => Error.Conflict("WorkOrder_RepairTask_AlreadyAdded", "Repair task already exists.");
    public static Error InvalidStateTransitionTime => Error.Conflict("WorkOrder_StateTransitionTime_Invalid", "State transition is not allowed before the work order’s scheduled start time.");
}
