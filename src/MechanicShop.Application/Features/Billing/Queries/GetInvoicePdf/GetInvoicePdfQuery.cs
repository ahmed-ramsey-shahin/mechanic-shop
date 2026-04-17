using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.Billing.Queries.GetInvoicePdf
{
    public sealed record GetInvoicePdfQuery(Guid InvoiceId) : ICachedQuery<Result<InvoicePdfDto>>
    {
        public string CacheKey => $"invoice_pdf:{InvoiceId}";

        public string[] Tags => ["invoice"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
