using FluentValidation;

namespace MechanicShop.Application.Features.Billing.Commands.SettleInvoice
{
    public class SettleInvoiceCommandValidator : AbstractValidator<SettleInvoiceCommand>
    {
        public SettleInvoiceCommandValidator()
        {
            RuleFor(command => command.InvoiceId)
                .NotEmpty()
                .WithMessage("Id is required");
        }
    }
}
