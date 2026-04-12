using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Domain.Employees;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.WorkOrders.Billing;
using MechanicShop.Domain.WorkOrders.Enums;

namespace MechanicShop.Domain.WorkOrders;

public sealed class WorkOrder : AuditableEntity
{
    public Guid VehicleId { get; private set; }
    public DateTimeOffset StartAt { get; private set; }
    public DateTimeOffset EndAt { get; private set; }
    public Guid LaborId { get; private set; }
    public Spot Spot { get; private set; }
    public WorkOrderState State { get; private set; }
    public Employee? Employee { get; set; }
    public Vehicle? Vehicle { get; set; }
    public Invoice? Invoice { get; set; }
    public decimal Discount { get; private set; }
    public decimal Tax { get; private set; }
    public decimal TotalPartsCost => _repairTasks.SelectMany(rt => rt.Parts).Sum(p => p.Cost);
    public decimal TotalLaborCost => _repairTasks.Sum(rt => rt.LaborCost);
    public bool IsEditable => State is not (WorkOrderState.Completed or WorkOrderState.Cancelled);
    private readonly List<RepairTask> _repairTasks = [];
    private IEnumerable<RepairTask> RepairTasks => _repairTasks.AsReadOnly();

    private WorkOrder()
    {
    }

    private WorkOrder(Guid id, Guid vehicleId, DateTimeOffset startAt, DateTimeOffset endAt, Guid laborId, Spot spot, WorkOrderState state, List<RepairTask> repairTasks) : base(id)
    {
        VehicleId = vehicleId;
        StartAt = startAt;
        EndAt = endAt;
        LaborId = laborId;
        Spot = spot;
        State = state;
        _repairTasks = repairTasks;
    }

    private static Result<bool> Validate(Guid vehicleId, DateTimeOffset startAt, DateTimeOffset endAt, Guid laborId, List<RepairTask> repairTasks)
    {
        if (vehicleId == Guid.Empty)
        {
            return WorkOrderErrors.VehicleIdRequired;
        }

        if (repairTasks == null || repairTasks.Count == 0)
        {
            return WorkOrderErrors.RepairTasksRequired;
        }

        if (laborId == Guid.Empty)
        {
            return WorkOrderErrors.LaborIdRequired;
        }

        if (endAt <= startAt)
        {
            return WorkOrderErrors.InvalidTiming;
        }
        return true;
    }

    public static Result<WorkOrder> Create(Guid id, Guid vehicleId, DateTimeOffset startAt, DateTimeOffset endAt, Guid laborId, Spot spot, WorkOrderState state, List<RepairTask> repairTasks)
    {
        var validationResult = Validate(vehicleId, startAt, endAt, laborId, repairTasks);
        if (validationResult.IsError)
        {
            return validationResult.Errors;
        }
        return new WorkOrder(id, vehicleId, startAt, endAt, laborId, spot, state, repairTasks);
    }

    public Result<Updated> AddRepairTask(RepairTask repairTask)
    {
        if (!IsEditable)
        {
            return WorkOrderErrors.Readonly;
        }

        if (_repairTasks.Any(r => r.Id == repairTask.Id))
        {
            return WorkOrderErrors.RepairTaskAlreadyAdded;
        }

        _repairTasks.Add(repairTask);
        return Result.Updated;
    }

    public Result<Updated> UpdateTiming(DateTimeOffset startAt, DateTimeOffset endAt)
    {
        if (!IsEditable)
        {
            return WorkOrderErrors.TimingReadonly(State);
        }

        if (endAt <= startAt)
        {
            return WorkOrderErrors.InvalidTiming;
        }

        StartAt = startAt;
        EndAt = endAt;
        return Result.Updated;
    }

    public Result<Updated> UpdateLabor(Guid laborId)
    {
        if (!IsEditable)
        {
            return WorkOrderErrors.Readonly;
        }

        if (laborId == Guid.Empty)
        {
            return WorkOrderErrors.LaborIdRequired;
        }

        LaborId = laborId;
        return Result.Updated;
    }

    public bool CanTransitionTo(WorkOrderState workOrderState)
    {
        return (State, workOrderState) switch
        {
            (WorkOrderState.Scheduled, WorkOrderState.InProgress) => true,
            (WorkOrderState.InProgress, WorkOrderState.Completed) => true,
            (_, WorkOrderState.Cancelled) => true,
            _ => false,
        };
    }

    public Result<Updated> UpdateState(WorkOrderState workOrderState)
    {
        if (!CanTransitionTo(workOrderState))
        {
            return WorkOrderErrors.InvalidStateTransition;
        }

        State = workOrderState;
        return Result.Updated;
    }

    public Result<Updated> Cancle()
    {
        return UpdateState(WorkOrderState.Cancelled);
    }

    public Result<Updated> ClearRepairTasks()
    {
        if (!IsEditable)
        {
            return WorkOrderErrors.Readonly;
        }

        _repairTasks.Clear();
        return Result.Updated;
    }

    public Result<Updated> UpdateSpot(Spot spot)
    {
        if (!IsEditable)
        {
            return WorkOrderErrors.Readonly;
        }

        Spot = spot;
        return Result.Updated;
    }
}
