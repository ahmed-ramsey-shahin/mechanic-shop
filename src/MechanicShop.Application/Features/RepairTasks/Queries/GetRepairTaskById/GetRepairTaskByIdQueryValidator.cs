using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById
{
    public class GetRepairTaskByIdQueryValidator : AbstractValidator<GetRepairTaskByIdQuery>
    {
        public GetRepairTaskByIdQueryValidator()
        {
            RuleFor(query => query.RepairTaskId)
                .NotEmpty()
                .WithMessage("Id is required");
        }
    }
}
