using Common.Authentication;
using Microsoft.Kiota.Abstractions.Authentication;

public class BearerTokenProvider(IKeycloakClientApplicationToken tokenRequester) : IAccessTokenProvider
{
    private readonly IKeycloakClientApplicationToken _tokenRequester = tokenRequester;

    public AllowedHostsValidator AllowedHostsValidator { get; }

    public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object> additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
    {

        var tokenResponse = await _tokenRequester
           .GetApplicationTokenAsync();
        return tokenResponse;
    }
}