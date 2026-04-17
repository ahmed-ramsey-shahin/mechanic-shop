using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTasks
{
    public class GetRepairTasksQueryValidator : AbstractValidator<GetRepairTasksQuery>
    {
        public GetRepairTasksQueryValidator()
        {
            RuleFor(query => query.PageSize).InclusiveBetween(5, 50).WithMessage("Page size must be between 5 and 50");
        }
    }
}
