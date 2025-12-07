using BankingAppDDD.Common.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BankingAppDDD.Common.Handlers
{
    public class RolesAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>, IAuthorizationHandler
    {
        private readonly ILogger<RolesAuthorizationHandler> _logger;
        public RolesAuthorizationHandler(ILogger<RolesAuthorizationHandler> logger)
        {
            _logger = logger;
        }
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
                var roleList = claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                var roles = requirement.AllowedRoles;
                var inputRole = roles.First();
                _logger.LogInformation("input role: {@inputrole}", inputRole);
                _logger.LogInformation("keycloak rolelist: {@rolelist}", roleList);
                if (roleList.Contains(inputRole))
                {
                    validRole = true;
                }
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
