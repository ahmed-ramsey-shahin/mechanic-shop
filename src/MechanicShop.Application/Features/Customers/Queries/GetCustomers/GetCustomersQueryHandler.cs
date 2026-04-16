using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers;

public sealed class GetCustomersQueryHandler(IAppDbContext context) : IRequestHandler<GetCustomersQuery, Result<PaginatedList<CustomerDto>>>
{
    private readonly IAppDbContext _db = context;

    public async Task<Result<PaginatedList<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _db.Customers.CountAsync(cancellationToken);
        var totalPages = (int) Math.Ceiling(totalCount / (double) request.PageSize);
        var customers = await _db.Customers.Include(customer => customer.Vehicles)
            .AsNoTracking()
            .Select(customer => customer.ToDto())
            .Skip((1 - request.Page) * request.PageSize)
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
