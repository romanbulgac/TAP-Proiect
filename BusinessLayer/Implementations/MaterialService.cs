using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class MaterialService : IMaterialService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;

        public MaterialService(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Guid> CreateMaterialAsync(MaterialDto materialDto, Guid userId)
        {
            // Basic authorization: check if userId matches teacherId or if user is admin
            // For simplicity, assuming materialDto.TeacherId is set correctly by caller
            // or derived from userId if user is a teacher.
            var material = _mapper.Map<Material>(materialDto);
            material.TeacherId = materialDto.TeacherId; // Ensure this is set, potentially from userId if teacher

            _context.Materials.Add(material);
            await _context.SaveChangesAsync();
            return material.Id;
        }

        public async Task<bool> DeleteMaterialAsync(Guid materialId, Guid userId)
        {
            var material = await _context.Materials.FindAsync(materialId);
            if (material == null) return false;

            // Authorization: check if user owns the material or is admin
            if (material.TeacherId != userId /* && !user.IsInRole("Admin") */) // Add role check if needed
            {
                // throw new UnauthorizedAccessException("User is not authorized to delete this material.");
                return false; 
            }

            // Soft delete
            material.IsDeleted = true;
            material.DeletedAt = DateTime.UtcNow;
            // _context.Materials.Update(material); // EF Core tracks changes
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MaterialDto?> GetMaterialByIdAsync(Guid id)
        {
            var material = await _context.Materials
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            return _mapper.Map<MaterialDto>(material);
        }

        public async Task<IEnumerable<MaterialDto>> GetMaterialsByTeacherAsync(Guid teacherId)
        {
            var materials = await _context.Materials
                .Where(m => m.TeacherId == teacherId)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<MaterialDto>>(materials);
        }
        
        public async Task<IEnumerable<MaterialDto>> GetMaterialsForConsultationAsync(Guid consultationId)
        {
            var materials = await _context.Materials
                .Where(m => m.ConsultationId == consultationId)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<MaterialDto>>(materials);
        }


        public async Task<bool> UpdateMaterialAsync(Guid materialId, MaterialDto materialDto, Guid userId)
        {
            var material = await _context.Materials.FindAsync(materialId);
            if (material == null) return false;

            if (material.TeacherId != userId /* && !user.IsInRole("Admin") */)
            {
                // throw new UnauthorizedAccessException("User is not authorized to update this material.");
                return false;
            }

            _mapper.Map(materialDto, material); // Update existing entity
            material.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
