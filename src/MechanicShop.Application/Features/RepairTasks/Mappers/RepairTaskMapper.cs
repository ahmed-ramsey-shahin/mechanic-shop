using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Parts;

namespace MechanicShop.Application.Features.RepairTasks.Mappers
{
    public static class RepairTaskMapper
    {
        public static PartDto ToDto(this Part part)
        {
            ArgumentNullException.ThrowIfNull(part);
            return new PartDto
            {
                Name = part.Name,
                Cost = part.Cost,
                Quantity = part.Quantity,
                PartId = part.Id,
            };
        }

        public static List<PartDto> ToDto(this IEnumerable<Part> parts)
        {
            return [.. parts.Select(part => part.ToDto())];
        }

        public static RepairTaskDto ToDto(this RepairTask repairTask)
        {
            ArgumentNullException.ThrowIfNull(repairTask);
            return new RepairTaskDto
            {
                RepairTaskId = repairTask.Id,
                LaborCost = repairTask.LaborCost,
                Name = repairTask.Name,
                Parts = repairTask.Parts.ToDto(),
                RepairDurationInMinutes = repairTask.RepairDurationInMinutes,
                TotalCost = repairTask.TotalCost
            };
        }

        public static List<RepairTaskDto> ToDto(this IEnumerable<RepairTask> repairTasks)
        {
            return [.. repairTasks.Select(task => task.ToDto())];
        }
    }
}
