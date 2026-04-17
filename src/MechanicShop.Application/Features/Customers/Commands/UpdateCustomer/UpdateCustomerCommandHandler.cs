using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandHandler(
        ILogger<UpdateCustomerCommandHandler> logger,
        IAppDbContext context,
        HybridCache cache
    ) : IRequestHandler<UpdateCustomerCommand, Result<Updated>>
    {
        private readonly ILogger<UpdateCustomerCommandHandler> _logger = logger;
        private readonly IAppDbContext _context = context;
        private readonly HybridCache _cache = cache;

        public async Task<Result<Updated>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers
                .Include(customer => customer.Vehicles)
                .FirstOrDefaultAsync(customer => customer.Id == request.CustomerId, cancellationToken);
            if (customer is null)
            {
                _logger.LogWarning("Customer {CustomerId} not found for update", request.CustomerId);
                return ApplicationErrors.CustomerNotFound;
            }
            List<Vehicle> ValidatedVehicles = [];
            foreach (var v in request.Vehicles)
            {
                var vehicleId = v.VehicleId ?? Guid.NewGuid();
                var vehicleResult = Vehicle.Create(vehicleId, v.Make, v.Model, v.Year, v.LicensePlate);
                if (vehicleResult.IsError)
                {
                    return vehicleResult.Errors;
                }
                ValidatedVehicles.Add(vehicleResult.Value);
            }
            var updateCustomerResult = customer.Update(request.Name, request.Email, request.PhoneNumber);
            if (updateCustomerResult.IsError)
            {
                return updateCustomerResult.Errors;
            }
            var syncVehiclesResult = customer.SynchronizeVehicles(ValidatedVehicles);
            if (syncVehiclesResult.IsError)
            {
                return syncVehiclesResult.Errors;
            }
            await _context.SaveChangesAsync(cancellationToken);
            await _cache.RemoveByTagAsync("customer", cancellationToken);
            return Result.Updated;
        }
    }
}
