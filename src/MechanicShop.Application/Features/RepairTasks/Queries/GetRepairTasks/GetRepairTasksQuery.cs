using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTasks
{
    public sealed record GetRepairTasksQuery(int Page, int PageSize) : ICachedQuery<Result<PaginatedList<RepairTaskDto>>>
    {
        public string CacheKey => $"repair_tasks:{Page}:{PageSize}";

        public string[] Tags => ["repair_task"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
