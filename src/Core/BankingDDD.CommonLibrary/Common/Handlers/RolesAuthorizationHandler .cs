using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
namespace BankingAppDDD.Common.Handlers
{
    public class RolesAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>, IAuthorizationHandler
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                      RolesAuthorizationRequirement requirement)
        {
            if (context.User is null || context.User.Identity is null)
                return Task.CompletedTask;
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var validRole = false;
            if (requirement.AllowedRoles == null ||
                requirement.AllowedRoles.Any() == false)
            {
                validRole = true;
            }
            else
            {
                var claims = context.User.Claims;
                var claimrole =claims.ElementAt(16);
                //var userName = claims?.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                var roles = requirement.AllowedRoles;
                var inputRole = roles.FirstOrDefault();
                if(claimrole.Value == inputRole)
                {
                    validRole = true;
                }
                //validRole = new Users().GetUsers(claims, roles).Where(p => roles.Contains(p.Role) && p.UserName == userName).Any();
            }

            if (validRole)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
