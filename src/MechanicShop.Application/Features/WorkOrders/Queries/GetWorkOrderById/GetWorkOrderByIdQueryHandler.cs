using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Application.Features.WorkOrders.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrderById
{
    public sealed class GetWorkOrderByIdQueryHandler(
        ILogger<GetWorkOrderByIdQueryHandler> logger,
        IAppDbContext context
    ) : IRequestHandler<GetWorkOrderByIdQuery, Result<WorkOrderDto>>
    {
        private readonly ILogger<GetWorkOrderByIdQueryHandler> _logger = logger;
        private readonly IAppDbContext _context = context;

        public async Task<Result<WorkOrderDto>> Handle(GetWorkOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var workOrder = await _context.WorkOrders
                .Include(order => order.RepairTasks).ThenInclude(task => task.Parts)
                .Include(order => order.Employee)
                .Include(order => order.Vehicle!).ThenInclude(vehicle => vehicle.Customer)
                .Include(order => order.Invoice)
                .AsNoTracking()
                .FirstOrDefaultAsync(order => order.Id == request.WorkOrderId, cancellationToken);
            if (workOrder is null)
            {
                _logger.LogWarning("WorkOrder with id {WorkOrderId} was not found", request.WorkOrderId);
                return ApplicationErrors.WorkOrderNotFound;
            }
            return workOrder.ToDto();
        }
    }
}
