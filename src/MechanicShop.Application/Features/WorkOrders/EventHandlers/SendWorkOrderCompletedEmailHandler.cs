using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.EventHandlers
{
    public sealed class SendWorkOrderCompletedEmailHandler(
        INotificationService notificationService,
        IAppDbContext context,
        ILogger<SendWorkOrderCompletedEmailHandler> logger
    ) : INotificationHandler<WorkOrderCompleted>
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly IAppDbContext _context = context;
        private readonly ILogger<SendWorkOrderCompletedEmailHandler> _logger = logger;

        public async Task Handle(WorkOrderCompleted notification, CancellationToken cancellationToken)
        {
            var workOrder = await _context.WorkOrders
                .Include(order => order.Vehicle!).ThenInclude(vehicle => vehicle.Customer!)
                .AsNoTracking()
                .FirstOrDefaultAsync(order => order.Id == notification.WorkOrderId, cancellationToken);
            if (workOrder is null)
            {
                _logger.LogError("Work order with id: {WorkOrderId} does not exist", notification.WorkOrderId);
                return;
            }
            await _notificationService.SendEmailAsync(workOrder.Vehicle!.Customer!.Email!, cancellationToken);
            await _notificationService.SendSmsAsync(workOrder.Vehicle!.Customer!.PhoneNumber!, cancellationToken);
        }
    }
}
