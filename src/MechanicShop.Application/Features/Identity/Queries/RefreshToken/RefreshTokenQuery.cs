using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Identity.Queries.RefreshToken;

public record RefreshTokenQuery(string RefreshToken, string ExpiredToken) : IRequest<Result<TokenResponse>>;
