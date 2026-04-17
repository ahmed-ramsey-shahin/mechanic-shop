using FluentValidation;

namespace MechanicShop.Application.Features.Labors.Queries.GetLabors
{
    public class GetLaborsQueryValidator : AbstractValidator<GetLaborsQuery>
    {
        public GetLaborsQueryValidator()
        {
            RuleFor(query => query.PageSize).InclusiveBetween(5, 50).WithMessage("Page size must be between 5 and 50");
        }
    }
}
