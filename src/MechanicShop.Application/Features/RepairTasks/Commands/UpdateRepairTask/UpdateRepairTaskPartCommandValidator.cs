using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask
{
    public class UpdateRepairTaskPartCommandValidator : AbstractValidator<UpdateRepairTaskPartCommand>
    {
        public UpdateRepairTaskPartCommandValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("Part name is required.")
                .MaximumLength(100);
            RuleFor(command => command.Cost)
                .GreaterThan(0).WithMessage("Part cost must be greater than 0.");
            RuleFor(command => command.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.");
        }
    }
}
