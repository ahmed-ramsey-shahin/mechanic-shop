using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask
{
    public sealed record CreateRepairTaskCommand(
        string? Name,
        decimal LaborCost,
        int RepairTaskDurationInMinutes,
        List<CreateRepairTaskPartCommand> Parts
    ) : IRequest<Result<RepairTaskDto>>;
}
