using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Billing.Queries.GetInvoicePdf
{
    public sealed class GetInvoicePdfQueryHandler(
        ILogger<GetInvoicePdfQueryHandler> logger,
        IAppDbContext context,
        IInvoicePdfGenerator pdfGenerator
    ) : IRequestHandler<GetInvoicePdfQuery, Result<InvoicePdfDto>>
    {
        private readonly ILogger<GetInvoicePdfQueryHandler> _logger = logger;
        private readonly IAppDbContext _context = context;
        private readonly IInvoicePdfGenerator _pdfGenerator = pdfGenerator;

        public async Task<Result<InvoicePdfDto>> Handle(GetInvoicePdfQuery request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.AsNoTracking()
                .Include(invoice => invoice.LineItems)
                .FirstOrDefaultAsync(invoice => invoice.Id == request.InvoiceId, cancellationToken);
            if (invoice is null)
            {
                _logger.LogWarning("Invoice not found. InvoiceId {InvoiceId}", request.InvoiceId);
                return ApplicationErrors.InvoiceNotFound;
            }
            try
            {
                var pdfBytes = _pdfGenerator.Generate(invoice);
                return new InvoicePdfDto
                {
                    Content = pdfBytes,
                    FileName = $"invoice-{invoice.Id}.pdf"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate PDF for InvoiceId: {InvoiceId}", request.InvoiceId);
                return Error.Failure("An error occured while generating the invoice PDF");
            }
        }
    }
}
