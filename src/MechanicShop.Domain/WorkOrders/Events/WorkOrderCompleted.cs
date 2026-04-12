using MechanicShop.Domain.Common;

namespace MechanicShop.Domain.WorkOrders.Events;

public sealed class WorkOrderCompleted : IDomainEvent
{
    public Guid WorkOrderId { get; set; }
}
