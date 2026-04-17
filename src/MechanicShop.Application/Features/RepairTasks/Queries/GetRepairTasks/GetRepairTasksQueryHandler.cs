using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTasks
{
    public sealed class GetRepairTasksQueryHandler(IAppDbContext context, ILogger<GetRepairTasksQueryHandler> logger) : IRequestHandler<GetRepairTasksQuery, Result<PaginatedList<RepairTaskDto>>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetRepairTasksQueryHandler> _logger = logger;

        public async Task<Result<PaginatedList<RepairTaskDto>>> Handle(GetRepairTasksQuery request, CancellationToken cancellationToken)
        {
            var totalCount = await _context.RepairTasks.CountAsync(cancellationToken);
            var totalPages = (int) Math.Ceiling(totalCount / (double) request.PageSize);
            if (request.Page > totalPages)
            {
                _logger.LogError("Could not return repair tasks because the required page is invalid: Maximum number of pages {MaxNoPages}, Required Page {PageNumber}", totalPages, request.Page);
                return ApplicationErrors.InvalidPage;
            }
            var repairTasks = await _context.RepairTasks
                .Include(repairTask => repairTask.Parts)
                .AsNoTracking()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
            return new PaginatedList<RepairTaskDto>()
            {
                Items = repairTasks.ToDto(),
                PageSize = request.PageSize,
                PageNumber = request.Page,
                TotalCount = totalCount,
                TotalPages = totalPages,
            };
        }
    }
}
