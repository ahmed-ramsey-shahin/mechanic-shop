using FluentValidation;

namespace MechanicShop.Application.Features.Billing.Queries.GetInvoiceById
{
    public class GetInvoiceByIdQueryValidator : AbstractValidator<GetInvoiceByIdQuery>
    {
        public GetInvoiceByIdQueryValidator()
        {
            RuleFor(query => query.InvoiceId)
                .NotEmpty()
                .WithMessage("Id is required");
        }
    }
}
