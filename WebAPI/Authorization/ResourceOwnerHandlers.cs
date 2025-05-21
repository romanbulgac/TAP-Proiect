using BusinessLayer.DTOs;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPI.Authorization
{
    // Handler for material ownership validation
    public class MaterialOwnerHandler : ResourceOwnerHandler<MaterialDto>
    {
        protected override Guid GetResourceOwnerId(MaterialDto resource)
        {
            return resource.TeacherId;
        }
    }

    // Handler for consultation ownership validation
    public class ConsultationOwnerHandler : ResourceOwnerHandler<ConsultationDto>
    {
        protected override Guid GetResourceOwnerId(ConsultationDto resource)
        {
            return resource.TeacherId;
        }
    }

    // Handler for review ownership validation
    public class ReviewOwnerHandler : ResourceOwnerHandler<ReviewDto>
    {
        protected override Guid GetResourceOwnerId(ReviewDto resource)
        {
            return resource.StudentId;
        }
    }
}
