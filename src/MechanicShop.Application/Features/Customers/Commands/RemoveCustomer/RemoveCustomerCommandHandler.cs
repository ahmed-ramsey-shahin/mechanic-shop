using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.RemoveCustomer
{
    public sealed class RemoveCustomerCommandHandler(
        ILogger<RemoveCustomerCommandHandler> logger,
        IAppDbContext context,
        HybridCache cache
    ) : IRequestHandler<RemoveCustomerCommand, Result<Deleted>>
    {
        private readonly ILogger<RemoveCustomerCommandHandler> _logger = logger;
        private readonly IAppDbContext _context = context;
        private readonly HybridCache _cache = cache;

        public async Task<Result<Deleted>> Handle(RemoveCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(customer => customer.Id == request.CustomerId, cancellationToken);
            if (customer is null)
            {
                _logger.LogWarning("Customer with id {CustomerId} not found for deletion", request.CustomerId);
                return ApplicationErrors.CustomerNotFound;
            }
            var hasAssociatedWorkOrders = await _context.WorkOrders.Include(workOrder => workOrder.Vehicle)
                .Where(workOrder => workOrder.Vehicle != null)
                .AnyAsync(workOrder => workOrder.Vehicle!.CustomerId == request.CustomerId, cancellationToken);
            if (hasAssociatedWorkOrders)
            {
                _logger.LogWarning("Could not delete the customer with id {CustomerId} becfause he has associated work orders.", request.CustomerId);
                return CustomerErrors.CannotDeleteCustomerWithWorkOrders;
            }
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync(cancellationToken);
            await _cache.RemoveByTagAsync("customer", cancellationToken);
            _logger.LogInformation("Customer {CustomerId} deleted successfully", request.CustomerId);
            return Result.Deleted;
        }
    }
}
