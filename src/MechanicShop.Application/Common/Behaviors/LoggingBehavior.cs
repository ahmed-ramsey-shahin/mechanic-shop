using MechanicShop.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Common.Behaviors;

public class LoggingBehavior<TRequest>(ILogger<TRequest> logger, IUser user, IIdentityService identityService) : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger = logger;
    private readonly IUser _user = user;
    private readonly IIdentityService _identityService = identityService;

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _user.Id;
        string? userName = string.Empty;
        if (userId != Guid.Empty)
        {
            userName = await _identityService.GetUsernameAsync(userId, cancellationToken);
        }
        _logger.LogInformation("Request: {Name} {UserId} {Username} {@Request}", requestName, userId, userName, request);
    }
}
