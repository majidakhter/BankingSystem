using BankingAppDDD.Common.Authentication;
using System;
using System.Threading.Tasks;

namespace BankingAppDDD.Identity.Services
{
    public interface  IKeycloakAuthService
    {
        //Task SignUpAsync(Guid id, string email, string password, string role = Role.User);
        Task<JsonWebToken> SignInAsync(string userName, string password);
        //Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);         
    }
}