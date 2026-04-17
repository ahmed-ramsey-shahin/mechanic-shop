using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask
{
    public sealed class UpdateRepairTaskCommandHandler(
        ILogger<UpdateRepairTaskCommandHandler> logger,
        IAppDbContext context,
        HybridCache cache
    ) : IRequestHandler<UpdateRepairTaskCommand, Result<Updated>>
    {
        private readonly ILogger<UpdateRepairTaskCommandHandler> _logger = logger;
        private readonly IAppDbContext _context = context;
        private readonly HybridCache _cache = cache;

        public async Task<Result<Updated>> Handle(UpdateRepairTaskCommand request, CancellationToken cancellationToken)
        {
            var repairTask = await _context.RepairTasks
                .Include(task => task.Parts)
                .FirstOrDefaultAsync(task => task.Id == request.RepairTaskId, cancellationToken);
            if (repairTask is null)
            {
                _logger.LogWarning("Repair task {RepairTaskId} not found for update.", request.RepairTaskId);
                return ApplicationErrors.RepairTaskNotFound;
            }
            var validatedParts = new List<Part>();
            foreach (var part in request.Parts)
            {
                var partId = part.PartId ?? Guid.NewGuid();
                var partResult = Part.Create(partId, part.Name, part.Cost, part.Quantity);
                if (partResult.IsError)
                {
                    return partResult.Errors;
                }
                validatedParts.Add(partResult.Value);
            }
            var updatedRepairTaskResult = repairTask.Update(request.Name!, request.LaborCost, request.RepairTaskDurationInMinutes);
            if (updatedRepairTaskResult.IsError)
            {
                return updatedRepairTaskResult.Errors;
            }
            var syncPartsResult = repairTask.SynchronizeParts(validatedParts);
            if (syncPartsResult.IsError)
            {
                return syncPartsResult.Errors;
            }
            await _context.SaveChangesAsync(cancellationToken);
            await _cache.RemoveByTagAsync("repait_task", cancellationToken);
            return Result.Updated;
        }
    }
}
