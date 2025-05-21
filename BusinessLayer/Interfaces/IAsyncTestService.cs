using BusinessLayer.ModelDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IAsyncTestService
    {
        Task<IEnumerable<TestModelDto>> GetAllAsync();
        Task<TestModelDto> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(TestModelDto model);
        Task UpdateAsync(Guid id, TestModelDto model);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<TestModelDto>> GetAllIncludingDeletedAsync();
    }
}
