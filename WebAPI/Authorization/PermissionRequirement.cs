using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPI.Authorization
{
    public enum PermissionType
    {
        ViewConsultations,
        CreateConsultation,
        EditConsultation,
        DeleteConsultation,
        ViewMaterials,
        UploadMaterials,
        EditMaterials,
        DeleteMaterials,
        ViewReviews,
        CreateReview,
        EditReview,
        DeleteReview,
        ManageUsers,
        ViewStatistics
    }

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionType Permission { get; }

        public PermissionRequirement(PermissionType permission)
        {
            Permission = permission;
        }
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            PermissionRequirement requirement)
        {
            // Check user role and map to permissions
            if (context.User.IsInRole("Administrator"))
            {
                // Admin has all permissions
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            bool hasPermission = false;

            // Teacher permissions
            if (context.User.IsInRole("Teacher"))
            {
                hasPermission = requirement.Permission switch
                {
                    PermissionType.ViewConsultations => true,
                    PermissionType.CreateConsultation => true,
                    PermissionType.EditConsultation => true,
                    PermissionType.DeleteConsultation => true,
                    PermissionType.ViewMaterials => true,
                    PermissionType.UploadMaterials => true,
                    PermissionType.EditMaterials => true,
                    PermissionType.DeleteMaterials => true,
                    PermissionType.ViewReviews => true,
                    PermissionType.ViewStatistics => true,
                    _ => false
                };
            }
            // Student permissions
            else if (context.User.IsInRole("Student"))
            {
                hasPermission = requirement.Permission switch
                {
                    PermissionType.ViewConsultations => true,
                    PermissionType.ViewMaterials => true,
                    PermissionType.ViewReviews => true,
                    PermissionType.CreateReview => true,
                    PermissionType.EditReview => true, // Student can only edit their own reviews (checked in resource handler)
                    PermissionType.DeleteReview => true, // Student can only delete their own reviews (checked in resource handler)
                    _ => false
                };
            }

            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
