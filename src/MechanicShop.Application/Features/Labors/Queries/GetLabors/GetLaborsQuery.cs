using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Labors.Dtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.Labors.Queries.GetLabors
{
    public sealed record GetLaborsQuery(int Page, int PageSize) : ICachedQuery<Result<PaginatedList<LaborDto>>>
    {
        public string CacheKey => $"labors:{Page}:{PageSize}";
        public string[] Tags => ["labors"];
        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }

}
