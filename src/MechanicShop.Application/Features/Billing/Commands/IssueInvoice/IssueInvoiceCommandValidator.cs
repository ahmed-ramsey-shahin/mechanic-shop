using FluentValidation;

namespace MechanicShop.Application.Features.Billing.Commands.IssueInvoice
{
    public class IssueInvoiceCommandValidator : AbstractValidator<IssueInvoiceCommand>
    {
        public IssueInvoiceCommandValidator()
        {
            RuleFor(command => command.WorkOrderId)
                .NotEmpty()
                .WithMessage("Work order id is required");
        }
    }
}
