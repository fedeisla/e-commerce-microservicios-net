using StackExchange.Redis;

public class RedisRateLimiter
{
    private readonly IConnectionMultiplexer _redis;

    public RedisRateLimiter(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<bool> IsAllowedAsync(string clientId, int maxRequests, TimeSpan window)
    {
        var db = _redis.GetDatabase();
        string key = $"ratelimit:{clientId}";

        // Incrementa el contador en Redis de forma atómica y segura
        long currentRequests = await db.StringIncrementAsync(key);

        // Si es la primera petición de la ventana, le asignamos el tiempo de expiración (TTL)
        if (currentRequests == 1)
        {
            await db.KeyExpireAsync(key, window);
        }

        // Retorna true si está dentro del límite, false si lo superó
        return currentRequests <= maxRequests;
    }
}