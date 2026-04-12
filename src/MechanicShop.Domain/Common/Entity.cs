namespace MechanicShop.Domain.Common;

internal class Entity
{
    public Guid Id { get; protected init; }
    protected Entity()
    {
    }

    protected Entity(Guid id)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
    }
}
