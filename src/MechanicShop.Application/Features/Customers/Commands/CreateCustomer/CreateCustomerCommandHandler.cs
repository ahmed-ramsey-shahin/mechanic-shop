using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandHandler(
    IAppDbContext db,
    ILogger<CreateCustomerCommandHandler> logger,
    HybridCache cache
) : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly ILogger<CreateCustomerCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;
    private readonly IAppDbContext _db = db;

    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.Customers.AnyAsync(c => c.Email!.Trim().Equals(request.Email.Trim(), StringComparison.OrdinalIgnoreCase), cancellationToken);
        if (exists)
        {
            _logger.LogWarning("Customer creation aborted. Email already exists.");
            return CustomerErrors.CustomerExists;
        }
        List<Vehicle> vehicles = [];
        foreach (var vehicle in request.Vehicles)
        {
            var vehicleResult = Vehicle.Create(Guid.NewGuid(), vehicle.Make, vehicle.Model, vehicle.Year, vehicle.LicensePlate);
            if (vehicleResult.IsError)
            {
                return vehicleResult.Errors;
            }
            vehicles.Add(vehicleResult.Value);
        }
        var customerResult = Customer.Create(Guid.NewGuid(), request.Name, request.PhoneNumber, request.Email, vehicles);
        if (customerResult.IsError)
        {
            return customerResult.Errors;
        }
        _db.Customers.Add(customerResult.Value);
        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Customer created successfully. Id: {customerId}", customerResult.Value.Id);
        await _cache.RemoveByTagAsync("customer", cancellationToken);
        return customerResult.Value.ToDto();
    }
}
