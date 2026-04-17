using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers;

public sealed class GetCustomersQueryHandler(IAppDbContext context, ILogger<GetCustomersQueryHandler> logger) : IRequestHandler<GetCustomersQuery, Result<PaginatedList<CustomerDto>>>
{
    private readonly IAppDbContext _db = context;
    private readonly ILogger<GetCustomersQueryHandler> _logger = logger;

    public async Task<Result<PaginatedList<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _db.Customers.CountAsync(cancellationToken);
        var totalPages = (int) Math.Ceiling(totalCount / (double) request.PageSize);
        if (request.Page > totalPages)
        {
            _logger.LogError("Could not return customers because the required page is invalid: Maximum number of pages {MaxNoPages}, Required Page {PageNumber}", totalPages, request.Page);
            return ApplicationErrors.InvalidPage;
        }
        var customers = await _db.Customers.Include(customer => customer.Vehicles)
            .AsNoTracking()
            .Select(customer => customer.ToDto())
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        return new PaginatedList<CustomerDto>()
        {
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = customers,
            PageNumber = request.Page
        };
    }
}
