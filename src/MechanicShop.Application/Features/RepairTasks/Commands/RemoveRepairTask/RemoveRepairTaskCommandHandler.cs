using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Commands.RemoveRepairTask
{
    public sealed class RemoveRepairTaskCommandHandler(
        IAppDbContext context,
        ILogger<RemoveRepairTaskCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<RemoveRepairTaskCommand, Result<Deleted>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<RemoveRepairTaskCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;

        public async Task<Result<Deleted>> Handle(RemoveRepairTaskCommand request, CancellationToken cancellationToken)
        {
            var repairTask = await _context.RepairTasks.FirstOrDefaultAsync(repairTask => repairTask.Id == request.RepairTaskId, cancellationToken);
            if (repairTask is null)
            {
                _logger.LogWarning("Repair task {RepairTaskId} was not found", request.RepairTaskId);
                return ApplicationErrors.RepairTaskNotFound;
            }
            var isInUse = await _context.WorkOrders
                .AsNoTracking()
                .SelectMany(workOrder => workOrder.RepairTasks)
                .AnyAsync(repairTask => repairTask.Id == request.RepairTaskId, cancellationToken);
            if (isInUse)
            {
                _logger.LogWarning("Repair task {RepairTaskId} can not be deleted because it is used by a work order", request.RepairTaskId);
                return RepairTaskErrors.InUse;
            }
            _context.RepairTasks.Remove(repairTask);
            await _context.SaveChangesAsync(cancellationToken);
            await _cache.RemoveByTagAsync("repair_task", cancellationToken);
            _logger.LogInformation("Repair task {RepairTaskId} deleted successfully", request.RepairTaskId);
            return Result.Deleted;
        }
    }
}
