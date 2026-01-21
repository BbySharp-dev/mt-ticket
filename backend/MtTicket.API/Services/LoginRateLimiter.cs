using Microsoft.Extensions.Caching.Memory;

namespace MtTicket.API.Services;

public interface ILoginRateLimiter
{
    bool IsLimitReached(string key, int limit, TimeSpan window);
}

public class LoginRateLimiter : ILoginRateLimiter
{
    private readonly IMemoryCache _cache;

    public LoginRateLimiter(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool IsLimitReached(string key, int limit, TimeSpan window)
    {
        if (!_cache.TryGetValue(key, out int count))
        {
            count = 0;
        }

        count++;
        _cache.Set(key, count, window);

        return count > limit;
    }
}

