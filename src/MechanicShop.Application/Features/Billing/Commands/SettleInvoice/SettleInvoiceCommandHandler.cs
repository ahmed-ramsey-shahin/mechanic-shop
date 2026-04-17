using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Billing.Commands.SettleInvoice
{
    public sealed class SettleInvoiceCommandHandler(
        ILogger<SettleInvoiceCommandHandler> logger,
        HybridCache cache,
        IAppDbContext context,
        TimeProvider datetime
    ) : IRequestHandler<SettleInvoiceCommand, Result<Success>>
    {
        private readonly ILogger<SettleInvoiceCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;
        private readonly IAppDbContext _context = context;
        private readonly TimeProvider _datetime = datetime;

        public async Task<Result<Success>> Handle(SettleInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(invoice => invoice.Id == request.InvoiceId, cancellationToken);
            if (invoice is null)
            {
                _logger.LogWarning("Invoice {InvoiceId} was not found", request.InvoiceId);
                return ApplicationErrors.InvoiceNotFound;
            }
            var payInvoiceResult = invoice.MarkAsPaid(_datetime);
            if (payInvoiceResult.IsError)
            {
                _logger.LogWarning("Invoice payment failed for InvoiceId: {InvoiceId}. Errors {@Errors}", request.InvoiceId, payInvoiceResult.Errors);
                return payInvoiceResult.Errors;
            }
            await _context.SaveChangesAsync(cancellationToken);
            await _cache.RemoveByTagAsync("invoice", cancellationToken);
            _logger.LogInformation("Invoice {InvoiceId} successfully paid.", request.InvoiceId);
            return Result.Success;
        }
    }
}
