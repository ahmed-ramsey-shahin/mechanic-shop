using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Application.Features.Billing.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Billing.Queries.GetInvoiceById
{
    public sealed class GetInvoiceByIdQueryHandler(
        ILogger<GetInvoiceByIdQueryHandler> logger,
        IAppDbContext context
    ) : IRequestHandler<GetInvoiceByIdQuery, Result<InvoiceDto>>
    {
        private readonly ILogger<GetInvoiceByIdQueryHandler> _logger = logger;
        private readonly IAppDbContext _context = context;

        public async Task<Result<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .AsNoTracking()
                .Include(invoice => invoice.LineItems)
                .Include(invoice => invoice.WorkOrder!).ThenInclude(workOrder => workOrder.Vehicle!).ThenInclude(vehicle => vehicle.Customer)
                .FirstOrDefaultAsync(invoice => invoice.Id == request.InvoiceId, cancellationToken);
            if (invoice is null)
            {
                _logger.LogWarning("Invoice was not found. InvoiceId: {InvoiceId}", request.InvoiceId);
                return ApplicationErrors.InvoiceNotFound;
            }
            return invoice.ToDto();
        }
    }
}
