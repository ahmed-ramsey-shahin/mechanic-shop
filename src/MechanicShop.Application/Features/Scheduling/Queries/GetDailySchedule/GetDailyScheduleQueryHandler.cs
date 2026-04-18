using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Labors.Mappers;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Application.Features.Scheduling.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.Scheduling.Queries.GetDailySchedule
{
    public sealed class GetDailyScheduleQueryHandler(IAppDbContext context, TimeProvider datetime) : IRequestHandler<GetDailyScheduleQuery, Result<ScheduleDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly TimeProvider _datetime = datetime;

        private static string? FormatVehicleInfo(Vehicle vehicle)
        {
            return vehicle != null ? $"{vehicle.Make} | {vehicle.LicensePlate}" : null;
        }

        public async Task<Result<ScheduleDto>> Handle(GetDailyScheduleQuery request, CancellationToken cancellationToken)
        {
            var localStart = request.ScheduleDate.ToDateTime(TimeOnly.MinValue);
            var localEnd = localStart.AddDays(1);
            var utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, request.TimeZone);
            var utcEnd = TimeZoneInfo.ConvertTimeToUtc(localEnd, request.TimeZone);

            var workOrders = await _context.WorkOrders
                .Where(workOrder => workOrder.StartAt < utcEnd && workOrder.EndAt > utcStart && (workOrder.LaborId == request.LaborId))
                .Include(workOrder => workOrder.RepairTasks)
                .Include(workOrder => workOrder.Vehicle)
                .Include(workOrder => workOrder.Employee)
                .ToListAsync(cancellationToken);
            var now = TimeZoneInfo.ConvertTime(_datetime.GetUtcNow(), request.TimeZone);
            var result = new ScheduleDto
            {
                OnDate = request.ScheduleDate,
                EndOfDay = localEnd < now,
                Spots = [],
            };

            foreach (var spot in Enum.GetValues<Spot>())
            {
                var current = localStart;
                var slots = new List<AvailabilitySlotDto>();
                var workOrdersBySpot = workOrders
                    .Where(workOrder => workOrder.Spot == spot)
                    .OrderBy(workOrder => workOrder.StartAt)
                    .ToList();
                while (current < localEnd)
                {
                    var next = current.AddMinutes(15);
                    var startUtc = TimeZoneInfo.ConvertTimeToUtc(current, request.TimeZone);
                    var endUtc = TimeZoneInfo.ConvertTimeToUtc(next, request.TimeZone);
                    var workOrder = workOrdersBySpot.FirstOrDefault(workOrder => workOrder.StartAt < endUtc && workOrder.EndAt > startUtc);
                    if (workOrder != null)
                    {
                        if (!slots.Any(slot => slot.WorkOrderId == workOrder.Id))
                        {
                            slots.Add(new AvailabilitySlotDto
                            {
                                WorkOrderId = workOrder.Id,
                                Spot = spot,
                                StartAt = workOrder.StartAt,
                                EndAt = workOrder.EndAt,
                                Vehicle = FormatVehicleInfo(workOrder.Vehicle!),
                                IsOccupied = true,
                                Labor = workOrder.Employee!.ToDto(),
                                RepairTasks = [.. workOrder.RepairTasks.ToList().ToDto()],
                                WorkOrderLocked = workOrder.IsEditable,
                                State = workOrder.State,
                                IsAvailable = false,
                            });
                        }
                    }
                    else
                    {
                        slots.Add(new AvailabilitySlotDto
                        {
                            Spot = spot,
                            StartAt = startUtc,
                            EndAt = endUtc,
                            WorkOrderLocked = false,
                            IsAvailable = current >= now,
                        });
                    }
                    current = next;
                }
                result.Spots.Add(new SpotDto
                {
                    Spot = spot,
                    Slots = slots,
                });
            }

            return result;
        }
    }
}
