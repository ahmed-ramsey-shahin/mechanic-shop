using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer;

public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(command => command.Make).NotEmpty().MaximumLength(50);
        RuleFor(command => command.Model).NotEmpty().MaximumLength(50);
        RuleFor(command => command.LicensePlate).NotEmpty().MaximumLength(10);
    }
}
