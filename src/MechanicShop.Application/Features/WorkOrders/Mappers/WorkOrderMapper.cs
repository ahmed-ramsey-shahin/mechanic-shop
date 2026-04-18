using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Application.Features.Labors.Dtos;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Domain.WorkOrders;

namespace MechanicShop.Application.Features.WorkOrders.Mappers
{
    public static class WorkOrderMapper
    {
        public static WorkOrderListItemDto ToListItemDto(this WorkOrder workOrder)
        {
            ArgumentNullException.ThrowIfNull(workOrder);
            return new WorkOrderListItemDto
            {
                WorkOrderId = workOrder.Id,
                Spot = workOrder.Spot,
                StartAt = workOrder.StartAt,
                EndAt = workOrder.EndAt,
                Vehicle = workOrder.Vehicle!.ToDto(),
                Labor = workOrder.Employee is null ? null : $"{workOrder.Employee.FirstName} {workOrder.Employee.LastName}",
                State = workOrder.State,
                RepairTasks = [.. workOrder.RepairTasks.Select(repairTask => repairTask.Name!)],
            };
        }

        public static WorkOrderDto ToDto(this WorkOrder workOrder)
        {
            ArgumentNullException.ThrowIfNull(workOrder);
            return new WorkOrderDto
            {
                WorkOrderId = workOrder.Id,
                Spot = workOrder.Spot,
                StartAt = workOrder.StartAt,
                EndAt = workOrder.EndAt,
                Labor = workOrder.Employee is null ? null : new LaborDto
                {
                    LaborId = workOrder.Employee.Id,
                    Name = $"{workOrder.Employee.FirstName} {workOrder.Employee.LastName}",
                },
                RepairTasks = workOrder.RepairTasks.ToDto(),
                Vehicle = workOrder.Vehicle!.ToDto(),
                State = workOrder.State,
                TotalPartCost = workOrder.RepairTasks.SelectMany(repeaiorTask => repeaiorTask.Parts).Sum(part => part.Quantity * part.Cost),
                TotalLaborCost = workOrder.RepairTasks.Sum(repairTask => repairTask.LaborCost),
                TotalCost = workOrder.RepairTasks.Sum(task => task.TotalCost),
                TotalDurationInMinutes = workOrder.RepairTasks.Sum(task => task.RepairDurationInMinutes),
                InvoiceId = workOrder.Invoice?.Id,
                CreatedAt = workOrder.CreatedAt
            };
        }

        public static List<WorkOrderDto> ToDto(this IEnumerable<WorkOrder> workOrders)
        {
            return [.. workOrders.Select(order => order.ToDto())];
        }
    }
}
