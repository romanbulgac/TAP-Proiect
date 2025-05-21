using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using DataAccess.Models;
using DataAccess.Repository;

namespace BusinessLayer.Base
{
    /// <summary>
    /// Base class for async services that provides common CRUD operations with mapping to DTOs
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TDto">The DTO type</typeparam>
    public abstract class BaseAsyncService<TEntity, TDto> 
        where TEntity : class
        where TDto : class
    {
        protected readonly IAsyncRepository<TEntity> Repository;
        protected readonly IMapper Mapper;
        
        protected BaseAsyncService(IAsyncRepository<TEntity> repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }
        
        /// <summary>
        /// Gets an entity by ID and maps it to DTO
        /// </summary>
        public virtual async Task<TDto> GetByIdAsync(Guid id)
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Entity with ID {id} not found");
                
            return Mapper.Map<TDto>(entity);
        }
        
        /// <summary>
        /// Gets all entities and maps them to DTOs
        /// </summary>
        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            var entities = await Repository.GetAllAsync();
            return Mapper.Map<IEnumerable<TDto>>(entities);
        }
        
        /// <summary>
        /// Finds entities matching a predicate and maps them to DTOs
        /// </summary>
        public virtual async Task<IEnumerable<TDto>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = await Repository.FindAsync(predicate);
            return Mapper.Map<IEnumerable<TDto>>(entities);
        }
        
        /// <summary>
        /// Creates a new entity from DTO and returns its ID
        /// </summary>
        public virtual async Task<Guid> CreateAsync(TDto dto)
        {
            var entity = Mapper.Map<TEntity>(dto);
            await Repository.AddAsync(entity);
            await Repository.SaveChangesAsync();
            
            // Attempt to get the ID property by convention
            var idProperty = typeof(TEntity).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException($"Entity {typeof(TEntity).Name} does not have an Id property");
                
            return (Guid)idProperty.GetValue(entity);
        }
        
        /// <summary>
        /// Updates an entity from DTO
        /// </summary>
        public virtual async Task UpdateAsync(Guid id, TDto dto)
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Entity with ID {id} not found");
                
            Mapper.Map(dto, entity);
            await Repository.UpdateAsync(entity);
            await Repository.SaveChangesAsync();
        }
        
        /// <summary>
        /// Deletes an entity (performs soft delete if entity implements ISoftDelete)
        /// </summary>
        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Entity with ID {id} not found");
            
            if (entity is ISoftDelete)
            {
                await Repository.SoftDeleteAsync(entity);
            }
            else
            {
                await Repository.DeleteAsync(entity);
            }
            
            await Repository.SaveChangesAsync();
        }
        
        /// <summary>
        /// Gets all entities including soft deleted ones and maps them to DTOs
        /// </summary>
        public virtual async Task<IEnumerable<TDto>> GetAllIncludingDeletedAsync()
        {
            // This will only work for ISoftDelete entities
            if (!typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
                throw new InvalidOperationException($"Entity {typeof(TEntity).Name} does not implement ISoftDelete");
                
            var entities = await Repository.GetAllIncludingDeletedAsync();
            return Mapper.Map<IEnumerable<TDto>>(entities);
        }
    }
}
