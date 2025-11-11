using BankingAppDDD.Common.Messages;
using Newtonsoft.Json;

namespace BankingAppDDD.Identity.Messages.Commands
{
    public class SignIn : ICommand
    {
        public string Username { get; }
        public string Password { get; }

        [JsonConstructor]
        public SignIn(string userName, string password)
        {
            Username = userName;
            Password = password;
        }
    }
}