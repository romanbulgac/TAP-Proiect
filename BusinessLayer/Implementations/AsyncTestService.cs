using BusinessLayer.Base;
using BusinessLayer.Interfaces;
using BusinessLayer.ModelDTOs;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace BusinessLayer.Implementations
{
    public class AsyncTestService : BaseAsyncService<TestModel, TestModelDto>, IAsyncTestService
    {
        public AsyncTestService(IAsyncRepository<TestModel> repository, IMapper mapper) 
            : base(repository, mapper)
        {
        }
        
        // The BaseAsyncService already implements all the standard CRUD operations
        // We can add any custom business logic methods here
        
        // Example of a custom business method:
        public async Task<IEnumerable<TestModelDto>> SearchByNameAsync(string searchTerm)
        {
            var entities = await Repository.FindAsync(t => t.Name.Contains(searchTerm));
            return Mapper.Map<IEnumerable<TestModelDto>>(entities);
        }
    }
}
