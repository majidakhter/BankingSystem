namespace BankingAppDDD.KeyCloakClientLibrary.KeyCloakRestHelper
{
    public record UserCreationRequest(string Username, string Email, string FirstName, string LastName, string Password);
}
