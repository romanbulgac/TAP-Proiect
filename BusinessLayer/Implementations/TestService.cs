using BusinessLayer.Interfaces;
using BusinessLayer.ModelDTOs;
using DataAccess.Models;
using DataAccess.Repository;

namespace BusinessLayer.Implementations
{
    public class TestService : ITestService
    {
        private readonly IRepository<TestModel> _repository;
        public TestService(IRepository<TestModel> repository)
        {
            _repository = repository;
        }

        public IEnumerable<TestModelDto> GetAll()
        {
            var models = _repository.GetAll();
            return models.Select(m => new TestModelDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description
            });
        }

        public TestModelDto GetById(Guid id)
        {
            var model = _repository.GetById(id);
            if (model == null) return null;
            return new TestModelDto
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description
            };
        }

        public void Create(TestModelDto model)
        {
            var entity = new TestModel
            {
                Name = model.Name,
                Description = model.Description
            };
            _repository.Add(entity);
            _repository.SaveChanges();
        }

        public void Update(Guid id, TestModelDto model)
        {
            var entity = _repository.GetById(id);
            if (entity == null) throw new KeyNotFoundException("Entity not found");

            entity.Name = model.Name;
            entity.Description = model.Description;
            _repository.Update(entity);
            _repository.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var entity = _repository.GetById(id);
            if (entity == null) throw new KeyNotFoundException("Entity not found");

            _repository.Remove(entity);
            _repository.SaveChanges();
        }
    }
}
