using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrderById
{
    public class GetWorkOrderByIdQueryValidator : AbstractValidator<GetWorkOrderByIdQuery>
    {
        public GetWorkOrderByIdQueryValidator()
        {
            RuleFor(query => query.WorkOrderId)
                .NotEmpty().WithMessage("Id is required");
        }
    }
}
