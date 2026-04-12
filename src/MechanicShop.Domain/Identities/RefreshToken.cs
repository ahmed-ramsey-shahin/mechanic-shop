using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Identities;

public sealed class RefreshToken : AuditableEntity
{
    public string? Token { get; }
    public string? UserId { get; }
    public DateTimeOffset ExpiresOn { get; }

    private RefreshToken()
    {
    }

    private RefreshToken(Guid id, string token, string userId, DateTimeOffset expiresOn) : base(id)
    {
        Token = token;
        UserId = userId;
        ExpiresOn = expiresOn;
    }

    private static Result<bool> Validate(string token, string userId, DateTimeOffset expiresOn)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return RefreshTokenErrors.TokenRequired;
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            return RefreshTokenErrors.UserIdRequired;
        }

        if (expiresOn <= DateTimeOffset.UtcNow)
        {
            return RefreshTokenErrors.ExpiryInvalid;
        }

        return true;
    }

    public static Result<RefreshToken> Create(Guid id, string token, string userId, DateTimeOffset expiresOn)
    {
        var validationResult = Validate(token, userId, expiresOn);
        if (validationResult.IsError)
        {
            return validationResult.Errors;
        }
        return new RefreshToken(id, token, userId, expiresOn);
    }
}
