using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Billing.Enums;

namespace MechanicShop.Domain.WorkOrders.Billing;

public sealed class Invoice : AuditableEntity
{
    public Guid WorkOrderId { get; private set; }
    public DateTimeOffset IssuedAt { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal Subtotal => CalculateSubtotal(LineItems);
    public decimal Total => Subtotal - DiscountAmount + TaxAmount;
    public DateTimeOffset? PaidAt { get; private set; }
    public WorkOrder? WorkOrder { get; private set; }
    private readonly List<InvoiceLineItem> _lineItems = [];
    public IEnumerable<InvoiceLineItem> LineItems => _lineItems.AsReadOnly();
    public InvoiceStatus Status { get; private set; }

    private Invoice()
    {
    }

    private Invoice(Guid id, Guid workOrderId, DateTimeOffset issuedAt, List<InvoiceLineItem> lineItems, decimal discountAmount, decimal taxAmount) : base(id)
    {
        WorkOrderId = workOrderId;
        IssuedAt = issuedAt;
        DiscountAmount = discountAmount;
        Status = InvoiceStatus.Unpaid;
        TaxAmount = taxAmount;
        _lineItems = lineItems;
    }

    private static decimal CalculateSubtotal(IEnumerable<InvoiceLineItem> items)
    {
        return items.Sum(item => item.LineTotal);
    }
    private static Result<bool> Validate(Guid workOrderId, List<InvoiceLineItem> lineItems, decimal discountAmount)
    {
        if (workOrderId == Guid.Empty)
        {
            return InvoiceErrors.WorkOrderIdInvalid;
        }

        if (lineItems is null || lineItems.Count == 0)
        {
            return InvoiceErrors.LineItemsEmpty;
        }

        if (discountAmount < 0 || discountAmount > CalculateSubtotal(lineItems))
        {
            return InvoiceErrors.DiscountInvalid;
        }

        return true;
    }

    public static Result<Invoice> Create(Guid id, Guid workOrderId, DateTimeOffset issuedAt, List<InvoiceLineItem> lineItems, decimal discountAmount, decimal taxAmount)
    {
        var validationResult = Validate(workOrderId, lineItems, discountAmount);
        if (validationResult.IsError)
        {
            return validationResult.Errors;
        }
        return new Invoice(id, workOrderId, issuedAt, lineItems, discountAmount, taxAmount);
    }

    public Result<Updated> ApplyDiscount(decimal discountAmount)
    {
        if (Status != InvoiceStatus.Unpaid)
        {
            return InvoiceErrors.InvoiceLocked;
        }

        if (discountAmount < 0 || discountAmount > CalculateSubtotal(LineItems))
        {
            return InvoiceErrors.DiscountInvalid;
        }

        DiscountAmount = discountAmount;
        return Result.Updated;
    }

    public Result<Updated> MarkAsPaid(TimeProvider timeProvider)
    {
        if (Status != InvoiceStatus.Unpaid)
        {
            return InvoiceErrors.InvoiceLocked;
        }

        Status = InvoiceStatus.Paid;
        PaidAt = timeProvider.GetUtcNow();
        return Result.Updated;
    }
}
