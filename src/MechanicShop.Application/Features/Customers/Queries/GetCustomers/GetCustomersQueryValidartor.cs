using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers;

public class GetCustomersQueryValidator : AbstractValidator<GetCustomersQuery>
{
    public GetCustomersQueryValidator()
    {
        RuleFor(query => query.PageSize).InclusiveBetween(5, 50);
    }
}
