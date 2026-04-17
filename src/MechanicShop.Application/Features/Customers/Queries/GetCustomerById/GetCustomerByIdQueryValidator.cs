using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
    {
        public GetCustomerByIdQueryValidator()
        {
            RuleFor(command => command.CustomerId)
                .NotEmpty()
                .WithMessage("Id is required");
        }
    }
}
