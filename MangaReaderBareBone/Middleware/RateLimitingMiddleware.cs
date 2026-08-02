using System.Collections.Concurrent;

namespace MangaReaderBareBone.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RateLimitOptions _options;

        private static readonly ConcurrentDictionary<string, FixedWindowCounter> _globalCounters = new();
        private static readonly ConcurrentDictionary<string, FixedWindowCounter> _writeCounters = new();

        public RateLimitingMiddleware(RequestDelegate next, RateLimitOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            if (!IsWithinLimit(_globalCounters, ip, _options.GlobalPermitLimit))
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return;
            }

            if (IsWriteOperation(context.Request.Method) &&
                !IsWithinLimit(_writeCounters, ip, _options.WritePermitLimit))
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return;
            }

            await _next(context);
        }

        private static bool IsWriteOperation(string method) =>
            method is "POST" or "PUT" or "DELETE" or "PATCH";

        private static bool IsWithinLimit(ConcurrentDictionary<string, FixedWindowCounter> counters, string key, int limit)
        {
            var now = DateTime.UtcNow;
            var windowStart = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, DateTimeKind.Utc);

            var counter = counters.GetOrAdd(key, _ => new FixedWindowCounter { WindowStart = windowStart, Count = 0 });

            lock (counter)
            {
                if (counter.WindowStart < windowStart)
                {
                    counter.WindowStart = windowStart;
                    counter.Count = 0;
                }

                if (counter.Count >= limit)
                    return false;

                counter.Count++;
                return true;
            }
        }

        private class FixedWindowCounter
        {
            public DateTime WindowStart;
            public int Count;
        }
    }

    public static class RateLimitingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitingMiddleware>();
        }
    }
}
