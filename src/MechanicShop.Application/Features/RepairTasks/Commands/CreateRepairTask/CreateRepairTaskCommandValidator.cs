using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask
{
    public class CreateRepairTaskCommandValidator : AbstractValidator<CreateRepairTaskCommand>
    {
        public CreateRepairTaskCommandValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100);
            RuleFor(command => command.LaborCost)
                .GreaterThan(0).WithMessage("Labor cost must be greater than 0.");
            RuleFor(command => command.RepairTaskDurationInMinutes)
                .NotNull().WithMessage("Estimated duration is required.")
                .IsInEnum();
            RuleFor(command => command.Parts)
                .NotNull().WithMessage("Parts list cannot be null.")
                .Must(p => p.Count > 0).WithMessage("At least one part is required.");
            RuleForEach(command => command.Parts).SetValidator(new CreateRepairTaskPartCommandValidator());
        }
    }
}
