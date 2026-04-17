using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(command => command.CustomerId).NotEmpty().WithMessage("Id is required");
            RuleFor(command => command.Name).NotEmpty().WithMessage("Name is required").MaximumLength(100);
            RuleFor(command => command.Email).EmailAddress().WithMessage("Invalid email").MaximumLength(100);
            RuleFor(command => command.PhoneNumber).NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+?\d{7,15}$").WithMessage("Phone number must be 7-15 digits and may start with '+'");
            RuleFor(command => command.Vehicles).NotNull().WithMessage("Vehicle list can not be null")
                .Must(p => p.Count > 0).WithMessage("At least one vehicle is required");
            RuleForEach(command => command.Vehicles).SetValidator(new UpdateVehicleCommandValidator());
        }
    }
}
