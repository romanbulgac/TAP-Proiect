using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    /// <summary>
    /// Generic asynchronous repository interface for data access operations
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from class</typeparam>
    public interface IAsyncRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> SaveChangesAsync();
        
        // Methods to handle soft delete
        Task SoftDeleteAsync(T entity);
        Task<IReadOnlyList<T>> GetAllIncludingDeletedAsync();
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
}
