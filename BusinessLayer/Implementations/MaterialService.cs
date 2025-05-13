using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccess;
using DataAccess.Models;

namespace BusinessLayer.Implementations
{
    public class MaterialService : IMaterialService
    {
        private readonly MyDbContext _context;
        public MaterialService(MyDbContext context) => _context = context;

        public List<MaterialDto> GetAll()
        {
            return _context.Materials
                .Select(m => new MaterialDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    ResourceUri = m.ResourceUri,
                    FileType = m.FileType,
                    Description = m.Description
                })
                .ToList();
        }

        public MaterialDto? GetById(Guid id)
        {
            var mat = _context.Materials.FirstOrDefault(m => m.Id == id);
            if (mat == null) return null;
            return new MaterialDto
            {
                Id = mat.Id,
                Title = mat.Title,
                ResourceUri = mat.ResourceUri,
                FileType = mat.FileType,
                Description = mat.Description
            };
        }

        public void Create(MaterialDto dto)
        {
            var entity = new Material
            {
                Title = dto.Title,
                ResourceUri = dto.ResourceUri,
                FileType = dto.FileType,
                Description = dto.Description
            };
            _context.Materials.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Guid id, MaterialDto dto)
        {
            var mat = _context.Materials.FirstOrDefault(m => m.Id == id);
            if (mat == null) return;
            mat.Title = dto.Title;
            mat.ResourceUri = dto.ResourceUri;
            mat.FileType = dto.FileType;
            mat.Description = dto.Description;
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var mat = _context.Materials.FirstOrDefault(m => m.Id == id);
            if (mat == null) return;
            _context.Materials.Remove(mat);
            _context.SaveChanges();
        }
    }
}
