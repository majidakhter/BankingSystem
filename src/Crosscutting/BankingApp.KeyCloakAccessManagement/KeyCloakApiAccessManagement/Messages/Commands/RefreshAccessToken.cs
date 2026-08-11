using BankingAppDDD.Common.Messages;
using Newtonsoft.Json;

namespace BankingAppDDD.Identity.Messages.Commands
{
    public class RefreshAccessToken : ICommand
    {
        public string Token { get; }

        [JsonConstructor]
        public RefreshAccessToken(string token)
        {
            Token = token;
        }
    }
}