using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.RemoveRepairTask
{
    public class RemoveRepairTaskCommandValidator : AbstractValidator<RemoveRepairTaskCommand>
    {
        public RemoveRepairTaskCommandValidator()
        {
            RuleFor(command => command.RepairTaskId).NotEmpty().WithMessage("RepairTaskId is required");
        }
    }
}
