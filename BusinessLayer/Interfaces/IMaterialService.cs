using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IMaterialService
    {
        Task<MaterialDto?> GetMaterialByIdAsync(Guid id);
        Task<IEnumerable<MaterialDto>> GetMaterialsForConsultationAsync(Guid consultationId);
        Task<IEnumerable<MaterialDto>> GetMaterialsByTeacherAsync(Guid teacherId);
        Task<Guid> CreateMaterialAsync(MaterialDto materialDto, Guid userId); // userId for authorization/ownership
        Task<bool> UpdateMaterialAsync(Guid materialId, MaterialDto materialDto, Guid userId);
        Task<bool> DeleteMaterialAsync(Guid materialId, Guid userId);
        // Add other methods as needed, e.g., for file uploads associated with materials
    }
}
