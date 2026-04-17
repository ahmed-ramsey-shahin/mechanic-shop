using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Labors.Dtos;
using MechanicShop.Application.Features.Labors.Mappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Identities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Labors.Queries.GetLabors
{
    public sealed class GetLaborsQueryHandler(IAppDbContext context, ILogger<GetLaborsQueryHandler> logger) : IRequestHandler<GetLaborsQuery, Result<PaginatedList<LaborDto>>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetLaborsQueryHandler> _logger = logger;

        public async Task<Result<PaginatedList<LaborDto>>> Handle(GetLaborsQuery request, CancellationToken cancellationToken)
        {
            var totalCount = await _context.Employees
                .AsNoTracking()
                .Where(e => e.Role == Role.Labor)
                .CountAsync(cancellationToken);
            var totalPages = (int) Math.Ceiling(totalCount / (double)request.PageSize);
            if (request.Page > totalPages)
            {
                _logger.LogError("Could not return labors because the required page is invalid: Maximum number of pages {MaxNoPages}, Required Page {PageNumber}", totalPages, request.Page);
                return ApplicationErrors.InvalidPage;
            }
            var labors = await _context.Employees
                .AsNoTracking()
                .Where(e => e.Role == Role.Labor)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
            return new PaginatedList<LaborDto>
            {
                PageSize = request.PageSize,
                PageNumber = request.Page,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = labors.ToDto(),
            };
        }
    }
}
