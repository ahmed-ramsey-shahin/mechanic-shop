using FluentValidation;

namespace MechanicShop.Application.Features.Billing.Queries.GetInvoicePdf
{
    public class GetInvoicePdfQueryValidator : AbstractValidator<GetInvoicePdfQuery>
    {
        public GetInvoicePdfQueryValidator()
        {
            RuleFor(query => query.InvoiceId)
                .NotEmpty()
                .WithMessage("Id is required");
        }
    }
}
