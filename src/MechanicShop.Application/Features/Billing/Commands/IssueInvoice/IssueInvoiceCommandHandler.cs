using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Application.Features.Billing.Mappers;
using MechanicShop.Domain.Common.Constants;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Billing;
using MechanicShop.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Billing.Commands.IssueInvoice
{
    public sealed class IssueInvoiceCommandHandler(
        ILogger<IssueInvoiceCommandHandler> logger,
        IAppDbContext context,
        HybridCache cache,
        TimeProvider datetime
    ) : IRequestHandler<IssueInvoiceCommand, Result<InvoiceDto>>
    {
        private readonly ILogger<IssueInvoiceCommandHandler> _logger = logger;
        private readonly IAppDbContext _context = context;
        private readonly HybridCache _cache = cache;
        private readonly TimeProvider _datetime = datetime;

        public async Task<Result<InvoiceDto>> Handle(IssueInvoiceCommand request, CancellationToken cancellationToken)
        {
            var workOrder = await _context.WorkOrders
                .Include(workOrder => workOrder.Vehicle!)
                .ThenInclude(vehicle => vehicle.Customer)
                .Include(workOrder => workOrder.RepairTasks)
                .ThenInclude(task => task.Parts)
                .FirstOrDefaultAsync(workOrder => workOrder.Id == request.WorkOrderId, cancellationToken);
            if (workOrder is null)
            {
                _logger.LogWarning("Invoice issuance failed. Work order id {WorkOrderId} was not found.", request.WorkOrderId);
                return ApplicationErrors.WorkOrderNotFound;
            }
            if (workOrder.State != WorkOrderState.Completed)
            {
                _logger.LogWarning("Invoice issuance rejected. WorkOrder {WorkOrderId} is not completed.", request.WorkOrderId);
                return ApplicationErrors.WorkOrderMustBeCompletedForInvoicing;
            }
            var invoiceId = Guid.NewGuid();
            var lineItems = new List<InvoiceLineItem>();
            var lineNumber = 1;
            foreach (var (task, taskIndex) in workOrder.RepairTasks.Select((t, i) => (t, i + 1)))
            {
                var partSummary = task.Parts.Any() ? string.Join(Environment.NewLine, task.Parts.Select(p => $"\t . {p.Name} x{p.Quantity} @ {p.Cost:C}")) : "\t .No parts";
                var lineDescription =
                    $"{taskIndex}: {task.Name}{Environment.NewLine}" +
                    $" Labor = {task.LaborCost:C}{Environment.NewLine}" +
                    $" Parts:{Environment.NewLine}{partSummary}";
                var totalPartsCost = task.Parts.Sum(p => p.Cost * p.Quantity);
                var totalTaskCost = task.LaborCost + totalPartsCost;
                var lineItemResult = InvoiceLineItem.Create(invoiceId: invoiceId, lineNumber: lineNumber++, description: lineDescription, quantity: 1, unitPrice: totalTaskCost);
                if (lineItemResult.IsError)
                {
                    return lineItemResult.Errors;
                }
                lineItems.Add(lineItemResult.Value);
            }
            var subTotal = lineItems.Sum(item => item.LineTotal);
            var taxAmount = subTotal * MechanicShopConstants.TaxRate;
            var discountAmount = workOrder.Discount;
            var createInvoiceResult = Invoice.Create(
                id: invoiceId,
                workOrderId: request.WorkOrderId,
                issuedAt: _datetime.GetUtcNow(),
                lineItems: lineItems,
                discountAmount: discountAmount,
                taxAmount: taxAmount
            );
            if (createInvoiceResult.IsError)
            {
                _logger.LogWarning("Invoice creation failed for WorkOrderId: {WorkOrderId}. Errors: {@Errors}", request.WorkOrderId, createInvoiceResult.Errors);
                return createInvoiceResult.Errors;
            }
            await _context.Invoices.AddAsync(createInvoiceResult.Value, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await _cache.RemoveByTagAsync("invoice", cancellationToken);
            _logger.LogInformation("Invoice {InvoiceId} issued for WorkOrder: {WorkOrderId}", createInvoiceResult.Value.Id, request.WorkOrderId);
            return createInvoiceResult.Value.ToDto();
        }
    }
}
