using MechanicShop.Application.Features.Customers.Dtos;

namespace MechanicShop.Application.Features.Billing.Dtos
{
    public class InvoiceDto
    {
        public Guid InvoiceId { get; set; }
        public Guid WorkOrderId { get; set; }
        public DateTimeOffset IssuedAt { get; set; }
        public CustomerDto? Customer { get; set; }
        public VehicleDto? Vehicle { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public string? PaymentStatus { get; set; }
        public List<InvoiceLineItemDto> Items { get; set; } = [];
    }
}
