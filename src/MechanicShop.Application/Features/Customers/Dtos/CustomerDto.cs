namespace MechanicShop.Application.Features.Customers.Dtos;

public class CustomerDto
{
    public Guid CustomerId { get; init; }
    public string? Name { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public List<VehicleDto> Vehicles { get; init; } = [];
}
