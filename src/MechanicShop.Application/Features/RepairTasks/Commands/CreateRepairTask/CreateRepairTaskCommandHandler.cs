using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask
{
    public sealed class CreateRepairTaskCommandHandler(
        ILogger<CreateRepairTaskCommandHandler> logger,
        IAppDbContext context,
        HybridCache cache
    ) : IRequestHandler<CreateRepairTaskCommand, Result<RepairTaskDto>>
    {
        private readonly ILogger<CreateRepairTaskCommandHandler> _logger = logger;
        private readonly IAppDbContext _context = context;
        private readonly HybridCache _cache = cache;

        public async Task<Result<RepairTaskDto>> Handle(CreateRepairTaskCommand request, CancellationToken cancellationToken)
        {
            var nameExists = await _context.RepairTasks.AnyAsync(task => EF.Functions.Like(task.Name, request.Name), cancellationToken);
            if (nameExists)
            {
                _logger.LogWarning("Duplicate repair task name '{PartName}'", request.Name);
                return RepairTaskErrors.DuplicateName;
            }
            List<Part> parts = [];
            foreach (var part in request.Parts)
            {
                var partResult = Part.Create(Guid.NewGuid(), part.Name, part.Cost, part.Quantity);
                if (partResult.IsError)
                {
                    return partResult.Errors;
                }
                parts.Add(partResult.Value);
            }
            var createRepairTaskResult = RepairTask.Create(
                id: Guid.NewGuid(),
                name: request.Name!,
                laborCost: request.LaborCost,
                repairDurationInMinutes: request.RepairTaskDurationInMinutes,
                parts: parts
            );
            if (createRepairTaskResult.IsError)
            {
                return createRepairTaskResult.Errors;
            }
            await _context.RepairTasks.AddAsync(createRepairTaskResult.Value, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await _cache.RemoveByTagAsync("repair_task");
            return createRepairTaskResult.Value.ToDto();
        }
    }
}
