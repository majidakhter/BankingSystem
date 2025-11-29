namespace BankingAppDDD.Common.Authentication
{
    public interface IJwtHandler
    {
        Task<JsonWebToken> GetToken(string userName, string password);
        //JsonWebTokenPayload GetTokenPayload(string accessToken);
    }
}