using BusinessLayer.ModelDTOs;

namespace BusinessLayer.Interfaces
{
    public interface ITestService
    {
        IEnumerable<TestModelDto> GetAll();

        TestModelDto GetById(Guid id);

        void Create(TestModelDto model);

        void Update(Guid id, TestModelDto model);

        void Delete(Guid id);
    }
}
