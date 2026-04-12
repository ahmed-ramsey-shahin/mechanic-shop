namespace MechanicShop.Domain.Common;

internal interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken ct=default);
}
