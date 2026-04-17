using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.WorkOrders.Billing;

namespace MechanicShop.Application.Features.Billing.Mappers
{
    public static class InvoiceMapper
    {
        public static InvoiceLineItemDto ToDto(this InvoiceLineItem invoiceLineItem)
        {
            return new InvoiceLineItemDto
            {
                InvoiceId = invoiceLineItem.InvoiceId,
                LineNumber = invoiceLineItem.LineNumber,
                Description = invoiceLineItem.Description,
                Quantity = invoiceLineItem.Quantity,
                UnitPrice = invoiceLineItem.UnitPrice,
                LineTotal = invoiceLineItem.LineTotal
            };
        }

        public static InvoiceDto ToDto(this Invoice invoice)
        {
            ArgumentNullException.ThrowIfNull(invoice);
            return new InvoiceDto
            {
                InvoiceId = invoice.Id,
                WorkOrderId = invoice.WorkOrderId,
                Customer = invoice.WorkOrder!.Vehicle!.Customer!.ToDto(),
                Vehicle = invoice.WorkOrder!.Vehicle!.ToDto(),
                IssuedAt = invoice.IssuedAt,
                TaxAmount = invoice.TaxAmount,
                SubTotal = invoice.Subtotal,
                DiscountAmount = invoice.DiscountAmount,
                Total = invoice.Total,
                PaymentStatus = invoice.Status.ToString(),
                Items = [.. invoice.LineItems.Select(item => item.ToDto())],
            };
        }

        public static List<InvoiceDto> ToDto(this List<Invoice> invoices)
        {
            return [.. invoices.Select(invoice => invoice.ToDto())];
        }

        public static List<InvoiceLineItemDto> ToDto(this List<InvoiceLineItem> invoiceLineItems)
        {
            return [.. invoiceLineItems.Select(item => item.ToDto())];
        }
    }
}
