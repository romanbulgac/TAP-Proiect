namespace BusinessLayer.Interfaces
{
    public interface IMaterialService
    {
        List<DTOs.MaterialDto> GetAll();
        DTOs.MaterialDto? GetById(Guid id);
        void Create(DTOs.MaterialDto dto);
        void Update(Guid id, DTOs.MaterialDto dto);
        void Delete(Guid id);
    }
}
