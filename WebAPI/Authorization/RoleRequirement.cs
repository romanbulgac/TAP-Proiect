using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace WebAPI.Authorization
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public string[] AllowedRoles { get; }

        public RoleRequirement(params string[] allowedRoles)
        {
            AllowedRoles = allowedRoles;
        }
    }

    public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            // Check if user has any of the required roles
            foreach (var role in requirement.AllowedRoles)
            {
                if (context.User.IsInRole(role))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }
}
