using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Common.Behaviours;

public class CachingBehaviour<TRequest, TResponse>(
    HybridCache cache, ILogger<CachingBehaviour<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly HybridCache _cache = cache;
    private readonly ILogger<CachingBehaviour<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICachedQuery cachedQuery)
        {
            return await next(cancellationToken);
        }
        _logger.LogInformation("Checking cache for {RequestName}", typeof(TRequest).Name);
        return await _cache.GetOrCreateAsync(
            key: cachedQuery.CacheKey,
            factory: async ct =>
            {
                var result = await next(ct);
                if (result is IResult r && r.IsSuccess)
                {
                    return result;
                }
                return default!;
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = cachedQuery.Expiration,
            },
            tags: cachedQuery.Tags,
            cancellationToken: cancellationToken
        );
    }
}
