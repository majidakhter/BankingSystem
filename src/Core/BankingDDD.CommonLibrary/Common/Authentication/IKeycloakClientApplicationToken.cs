namespace Common.Authentication
{
    public interface IKeycloakClientApplicationToken
    {
        Task<string> GetApplicationTokenAsync();
    }
}
