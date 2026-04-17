using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers;

public class GetCustomersQueryValidator : AbstractValidator<GetCustomersQuery>
{
    public GetCustomersQueryValidator()
    {
        RuleFor(query => query.PageSize).InclusiveBetween(5, 50).WithMessage("Page size must be between 5 and 50");
    }
}
