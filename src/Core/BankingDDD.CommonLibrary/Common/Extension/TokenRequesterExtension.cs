using BankingAppDDD.Common.Authentication;
using BankingAppDDD.Common.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankingAppDDD.Common.Extension
{
    public static class TokenRequesterExtension
    {
        public static IServiceCollection ConfigureTokenRequester(this IServiceCollection services,
        IConfiguration configuration)
        {
            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration));
            var appSettings = configuration.Get<AppSettings>()
                 ?? throw new ArgumentNullException("AppSettings section was not found.");

            //services.AddRedis(configuration);
            services.AddHttpClient();
            services.AddTransient<IAccessTokenService, AccessTokenService>();
            services.AddTransient<AccessTokenValidatorMiddleware>();
            return services;
        }
    }
}
