using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.WorkOrders.Billing;

public static class InvoiceLineItemErrors
{
    public static Error InvoiceIdRequired => Error.Validation("InvoiceLineItem_InvoiceId_Required", "InvoiceId is required.");
    public static Error LineNumberInvalid => Error.Validation("InvoiceLineItem_LineNumberInvalid", "Line number must be greater than 0.");
    public static Error DescriptionRequired => Error.Validation("InvoiceLineItem_Description_Invalid", "Description is required.");
    public static Error QuantityInvalid => Error.Validation("InvoiceLineItem_Quantity_Invalid", "Quantity must be greater than 0.");
    public static Error UnitPriceInvalid => Error.Validation("InvoiceLineItem_UnitPrice_Invalid", "Unit price must be greater than 0.");
}
