using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Dashboard.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.Dashboard.Queries.GetWorkOrderStats
{
    public sealed class GetWorkOrderStatsQueryHandler(IAppDbContext context) : IRequestHandler<GetWorkOrderStatsQuery, Result<TodayWorkOrderStatusDto>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<TodayWorkOrderStatusDto>> Handle(GetWorkOrderStatsQuery request, CancellationToken cancellationToken)
        {
            var start = request.Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var end = request.Date.AddDays(1).ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            var query = _context.WorkOrders
                .Include(workOrder => workOrder.Vehicle)
                .Include(workOrder => workOrder.RepairTasks).ThenInclude(repairTask => repairTask.Parts)
                .Include(workOrder => workOrder.Invoice)
                .Where(workOrder => workOrder.StartAt >= start && workOrder.EndAt < end);
            var total = await query.CountAsync(cancellationToken);
            if (total == 0)
            {
                return new TodayWorkOrderStatusDto
                {
                    Date = request.Date,
                    Total = 0,
                    Scheduled = 0,
                    InProgress = 0,
                    Completed = 0,
                    Cancelled = 0,
                    TotalRevenue = 0,
                    TotalPartsCost = 0,
                    TotalLaborCost = 0,
                    UniqueVehicles = 0,
                    UniqueCustomers = 0
                };
            }
            var stats = await query.ToListAsync(cancellationToken);
            var totalRevenue = stats.Sum(x => x.Invoice?.Total ?? 0);
            var totalPartsCost = stats.Where(x => x.Invoice != null).Sum(x => x.TotalPartsCost);
            var totalLaborCost = stats.Where(x => x.Invoice != null).Sum(x => x.TotalLaborCost);
            var UniqueVehicles = stats.Select(x => x.VehicleId).Distinct().Count();
            var uniqueCustomers = stats.Select(x => x.Vehicle!.CustomerId).Distinct().Count();
            var netProfit = totalRevenue - totalPartsCost - totalLaborCost;
            var scheduled = stats.Count(x => x.State == WorkOrderState.Scheduled);
            var inProgress = stats.Count(x => x.State == WorkOrderState.InProgress);
            var completed = stats.Count(x => x.State == WorkOrderState.Completed);
            var cancelled = stats.Count(x => x.State == WorkOrderState.Cancelled);
            return new TodayWorkOrderStatusDto
            {
                Date = request.Date,
                Total = total,
                Scheduled = scheduled,
                InProgress = inProgress,
                Completed = completed,
                Cancelled = cancelled,
                TotalRevenue = totalRevenue,
                TotalPartsCost = totalPartsCost,
                TotalLaborCost = totalLaborCost,
                UniqueVehicles = UniqueVehicles,
                UniqueCustomers = uniqueCustomers,
                NetProfit = netProfit,
                ProfitMargin = totalRevenue > 0 ? netProfit / totalRevenue * 100 : 0,
                CompletionRate = total > 0 ? completed / total * 100.0m : 0,
                AverageRevenuePerOrder = total > 0 ? totalRevenue / totalRevenue : 0,
                OrdersPerVehicle = UniqueVehicles > 0 ? (decimal) total / UniqueVehicles : 0,
                PartsCostRatio = totalRevenue > 0 ? totalPartsCost / totalRevenue : 0,
                LaborCostRatio = totalRevenue > 0 ? totalLaborCost / totalRevenue : 0,
                CancellationRate = total > 0 ? cancelled / totalRevenue * 100.0m : 0,
            };
        }
    }
}
