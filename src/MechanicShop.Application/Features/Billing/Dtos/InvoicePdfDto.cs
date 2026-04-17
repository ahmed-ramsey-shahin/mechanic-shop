namespace MechanicShop.Application.Features.Billing.Dtos
{
    public class InvoicePdfDto
    {
        public byte[]? Content { get; set; }
        public string? FileName { get; set; }
        public string ContentType { get; set; } = "application/json";
    }
}
