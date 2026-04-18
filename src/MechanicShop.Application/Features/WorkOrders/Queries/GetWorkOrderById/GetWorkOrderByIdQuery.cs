using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrderById
{
    public sealed record GetWorkOrderByIdQuery(Guid WorkOrderId) : ICachedQuery<Result<WorkOrderDto>>
    {
        public string CacheKey => $"work_orders:{WorkOrderId}";

        public string[] Tags => ["work_order"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
