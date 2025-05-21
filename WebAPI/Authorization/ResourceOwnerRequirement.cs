using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPI.Authorization
{
    public class ResourceOwnerRequirement : IAuthorizationRequirement
    {
        public string[] AdminRoles { get; }

        public ResourceOwnerRequirement(params string[] adminRoles)
        {
            AdminRoles = adminRoles;
        }
    }

    public class ResourceOwnerHandler<T> : AuthorizationHandler<ResourceOwnerRequirement, T> where T : class
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            ResourceOwnerRequirement requirement, 
            T resource)
        {
            // If the user is in an admin role, always allow access
            foreach (var role in requirement.AdminRoles)
            {
                if (context.User.IsInRole(role))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            // Get user ID from claims
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Task.CompletedTask;
            }

            // Get the owner ID from the resource - this should be implemented by derived classes
            Guid resourceOwnerId = GetResourceOwnerId(resource);
            
            // If user ID matches resource owner ID, grant access
            if (Guid.TryParse(userIdClaim.Value, out Guid userId) && userId == resourceOwnerId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        protected virtual Guid GetResourceOwnerId(T resource)
        {
            // This should be overridden by derived classes
            return Guid.Empty;
        }
    }
}
