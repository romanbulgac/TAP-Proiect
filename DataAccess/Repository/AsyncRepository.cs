using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository
{
    /// <summary>
    /// Generic asynchronous repository implementation for data access operations
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from class</typeparam>
    public class AsyncRepository<T> : IAsyncRepository<T> where T : class
    {
        private readonly MyDbContext _context;
        private readonly DbSet<T> _entities;

        public AsyncRepository(MyDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entities.Where(predicate).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _entities;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _entities.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(T entity)
        {
            _entities.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(T entity)
        {
            // Check if entity implements ISoftDelete
            if (entity is ISoftDelete softDeleteEntity)
            {
                softDeleteEntity.IsDeleted = true;
                softDeleteEntity.DeletedAt = DateTime.UtcNow;
                _entities.Update(entity);
                await Task.CompletedTask;
            }
            else
            {
                throw new InvalidOperationException($"Entity of type {typeof(T).Name} does not implement ISoftDelete interface");
            }
        }

        public async Task<IReadOnlyList<T>> GetAllIncludingDeletedAsync()
        {
            // Only works for entities implementing ISoftDelete
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
            {
                return await _entities.IgnoreQueryFilters().ToListAsync();
            }
            else
            {
                throw new InvalidOperationException($"Entity of type {typeof(T).Name} does not implement ISoftDelete interface");
            }
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entities.AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entities.CountAsync(predicate);
        }
    }
}
