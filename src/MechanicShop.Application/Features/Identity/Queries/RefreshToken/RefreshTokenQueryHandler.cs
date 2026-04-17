using System.Security.Claims;
using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Identity.Queries.RefreshToken;

public class RefreshTokenQueryHandler(
    ILogger<RefreshTokenQueryHandler> logger,
    IIdentityService identityService,
    IAppDbContext context,
    ITokenProvider tokenProvider
) : IRequestHandler<RefreshTokenQuery, Result<TokenResponse>>
{
    private readonly ILogger<RefreshTokenQueryHandler> _logger = logger;
    private readonly IIdentityService _identityService = identityService;
    private readonly IAppDbContext _db = context;
    private readonly ITokenProvider _tokenProvider = tokenProvider;

    public async Task<Result<TokenResponse>> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var principal = _tokenProvider.GetPrincipalFromExpiredToken(request.ExpiredToken);
        if (principal is null)
        {
            _logger.LogInformation("Expired token is not valid");
            return ApplicationErrors.ExpiredAccessTokenInvalid;
        }
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            _logger.LogInformation("Invalid UserId Claim");
            return ApplicationErrors.UserIdClaimInvalid;
        }
        var getUserResult = await _identityService.GetUserByIdAsync(Guid.Parse(userId), cancellationToken);
        if (getUserResult.IsError)
        {
            _logger.LogError("Get user by id error occured: {Error Description}", getUserResult.TopError.Description);
            return getUserResult.Errors;
        }
        var refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(token => token.Token == request.RefreshToken && userId == token.UserId, cancellationToken);
        if (refreshToken is null || refreshToken.ExpiresOn < DateTimeOffset.UtcNow)
        {
            _logger.LogError("Refresh token has expired");
            return ApplicationErrors.RefreshTokenExpired;
        }
        var generateTokenResult = await _tokenProvider.GenerateJwtTokenAsync(getUserResult.Value, cancellationToken);
        if (generateTokenResult.IsError)
        {
            _logger.LogError("Generate token error occured: {ErrorDescription}", generateTokenResult.TopError.Description);
            return generateTokenResult.Errors;
        }
        return generateTokenResult.Value;
    }
}
