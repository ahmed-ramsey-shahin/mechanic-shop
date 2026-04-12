using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.WorkOrders.Billing;

public static class InvoiceErrors
{
    public static readonly Error WorkOrderIdInvalid = Error.Validation("Invoice_WorkOrderId_Invalid", "WorkOrderId is invalid");
    public static readonly Error LineItemsEmpty = Error.Validation("Invoice_LineItems_Empty", "Invoice must have line items");
    public static readonly Error InvoiceLocked = Error.Validation("Invoice_Locked", "Invoice is locked");
    public static readonly Error DiscountInvalid = Error.Validation("Invoice_Discount_Invalid", "Discount must be between 0 and subtotal");
}
