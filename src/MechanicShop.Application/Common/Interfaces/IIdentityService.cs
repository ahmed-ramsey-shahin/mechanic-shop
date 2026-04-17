using MechanicShop.Application.Features.Identity.Dtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellationToken);
    Task<bool> AuthorizeAsync(Guid userId, string? policyName, CancellationToken cancellationToken);
    Task<Result<AppUserDto>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken);
    Task<Result<AppUserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<string?> GetUsernameAsync(Guid userId, CancellationToken cancellationToken);
}
