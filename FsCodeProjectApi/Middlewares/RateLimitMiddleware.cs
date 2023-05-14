
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace FsCodeProjectApi.Middlewares
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RateLimitOptions _options;
        private readonly IMemoryCache _cache;

        public RateLimitMiddleware(RequestDelegate next, IOptions<RateLimitOptions> options, IMemoryCache cache)
        {
            _next = next;
            _options = options.Value;
            _cache = cache;
        }

        public async System.Threading.Tasks.Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress.ToString();
            var cacheKey = $"{ipAddress}-{context.Request.Path}";

            if (!_cache.TryGetValue(cacheKey, out int requests))
            {
                // First request, initialize the request count
                _cache.Set(cacheKey, 1, DateTimeOffset.UtcNow.Add(_options.TimeSpan = TimeSpan.FromMinutes(1)));
            }
            else
            {
                if (requests >= _options.Requests)
                {
                    // Rate limit exceeded, return an error response
                    context.Response.StatusCode = _options.StatusCode;
                    await context.Response.WriteAsync(_options.Message);
                    return;
                }

                // Increment the request count
                _cache.Set(cacheKey, requests + 1, DateTimeOffset.UtcNow.Add(_options.TimeSpan));
            }

            await _next(context);
        }
    }
}
