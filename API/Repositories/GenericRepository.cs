using API.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Contract;

namespace API.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<bool> AddAsync(T entity)
        {
            try
            {
                var addedEntity = await _dbSet.AddAsync(entity);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            try
            {
                _dbSet.Entry(entity).State = EntityState.Deleted;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity != null)
                {
                    await DeleteAsync(entity);
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                var user = await _dbSet.FindAsync(id);
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Entry(entity).State = EntityState.Detached;

                _dbSet.Attach(entity);

                _dbSet.Entry(entity).State = EntityState.Modified;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                var user = await _dbSet.ToListAsync();
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetAllByUserIdAsync(int id)
        {
            try
            {
                var user = await _dbSet.Where(e => EF.Property<int>(e, "UserId") == id).ToListAsync();
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void Dispose()
        {
            _context.Dispose();

        }

        public async Task<T> AddEntityAsync(T entity)
        {
            try
            {
                var addedEntity = await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync(); // Ensure ID is generated
                return addedEntity.Entity; // Return the entity with the populated ID
            }
            catch (Exception ex)
            {
                return null; // Return null if an error occurs
            }
        }
    }
}
