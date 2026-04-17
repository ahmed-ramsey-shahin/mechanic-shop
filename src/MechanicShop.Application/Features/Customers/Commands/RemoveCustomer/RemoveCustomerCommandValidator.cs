using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.RemoveCustomer
{
    public class RemoveCustomerCommandValidator : AbstractValidator<RemoveCustomerCommand>
    {
        public RemoveCustomerCommandValidator()
        {
            RuleFor(command => command.CustomerId)
                .NotEmpty().WithMessage("Customer Id is required.");
        }
    }
}
