using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contract
{
    public interface IGenericRepository<T> : IDisposable where T : class
    {
        public Task<IEnumerable<T>> GetAllAsync();

        public Task<IEnumerable<T>> GetAllByUserIdAsync(int id);

        public Task<bool> AddAsync(T entity);

        public Task<bool> UpdateAsync(T entity);

        public Task<bool> DeleteAsync(T entity);

        public Task<bool> DeleteByIdAsync(int id);

        public Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Add entity to database and return the created ID
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>return the ID of created entity</returns>
        public Task<T> AddEntityAsync(T entity);

        public Task SaveAsync();


        // New methods for transactions
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }

}
