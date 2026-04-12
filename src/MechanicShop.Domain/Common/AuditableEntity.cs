namespace MechanicShop.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset LastModified { get; set; }

    protected AuditableEntity()
    {
    }

    protected AuditableEntity(Guid id) : base(id)
    {
    }
}
