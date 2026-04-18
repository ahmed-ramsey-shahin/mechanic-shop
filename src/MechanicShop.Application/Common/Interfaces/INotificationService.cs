namespace MechanicShop.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task SendEmailAsync(string email, CancellationToken cancellationToken);
        Task SendSmsAsync(string phoneNumber, CancellationToken cancellationToken);
    }
}
