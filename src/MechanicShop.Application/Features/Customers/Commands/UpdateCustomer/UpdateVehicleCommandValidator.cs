using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer
{
    public class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
    {
        public UpdateVehicleCommandValidator()
        {
            RuleFor(command => command.Make).NotEmpty().MaximumLength(50);
            RuleFor(command => command.Model).NotEmpty().MaximumLength(50);
            RuleFor(command => command.LicensePlate).NotEmpty().MaximumLength(10);
            RuleFor(command => command.VehicleId).NotEmpty().WithMessage("Vehicle Id is required.");
        }
    }
}
