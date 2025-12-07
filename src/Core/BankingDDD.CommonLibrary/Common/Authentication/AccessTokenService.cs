using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace BankingAppDDD.Common.Authentication
{
    public class AccessTokenService : IAccessTokenService
    {
        private readonly IDistributedCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly ILogger<AccessTokenService> _logger;
        public AccessTokenService(IDistributedCache cache,
                IHttpContextAccessor httpContextAccessor,
                IOptions<JwtOptions> jwtOptions,
                ILogger<AccessTokenService> logger)
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _jwtOptions = jwtOptions;
            _logger = logger;
        }
        public async Task<bool> IsCurrentActiveToken()
            => await IsActiveAsync(GetCurrentAsync());

        public async Task DeactivateCurrentAsync(string userId)
            => await DeactivateAsync(userId, GetCurrentAsync());

        public async Task<bool> IsActiveAsync(string token)
            => string.IsNullOrWhiteSpace(await _cache.GetStringAsync(GetKey(token)));

        public async Task DeactivateAsync(string userId, string token)
        {
            await _cache.SetStringAsync(GetKey(token),
                    "deactivated", new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow =
                            TimeSpan.FromMinutes(_jwtOptions.Value.ExpiryMinutes)
                    });
        }

        private string GetCurrentAsync()
        {
            var authorizationHeader = _httpContextAccessor
                .HttpContext!.Request.Headers["authorization"] ;
            var value = authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(' ').Last();
           
                _logger.LogInformation("Created token: {@token}", value);
            return value;
        }

        private static string GetKey(string token)
            => $"tokens:{token}";
    }
}
