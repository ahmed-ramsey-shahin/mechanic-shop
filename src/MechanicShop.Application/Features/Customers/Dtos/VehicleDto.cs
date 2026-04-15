namespace MechanicShop.Application.Features.Customers.Dtos;

public class VehicleDto
{
    public Guid VehicleId { get; init; }
    public string? Make { get; init; }
    public string? Model { get; init; }
    public int Year { get; init; }
    public string? LicensePlate { get; init; }
}
