using Microsoft.Extensions.Caching.Memory;

namespace Ropufu.Homepage;

public class CacheOnceAgent<T>
{
    private readonly object _lock = new();

    public CacheOnceAgent(T fallbackValue) =>
        this.LatestValue = fallbackValue;

    public T LatestValue { get; private set; }

    public T UpdateOnce(IMemoryCache memoryCache, object key, MemoryCacheEntryOptions options, Func<T> factory)
    {
        if (!Monitor.TryEnter(this._lock))
            return this.LatestValue;

        T value = factory();
        if (value is null)
        {
            Monitor.Exit(this._lock);
            return this.LatestValue;
        } // if (...)

        memoryCache.Set(key, value, options);
        this.LatestValue = value;

        Monitor.Exit(this._lock);
        return value;
    }
}

public static class MemoryCacheExtender
{
    public static T GetOrCreateOnce<T>(this IMemoryCache memoryCache, object key, CacheOnceAgent<T> agent, MemoryCacheEntryOptions options, Func<T> factory)
    {
        // The value hasn't expired yet.
        if (memoryCache.TryGetValue(key, out T? value))
            return value!;

        // The value has expired: execute factory method for few unlucky users.
        return agent.UpdateOnce(memoryCache, key, options, factory);
    }
}
