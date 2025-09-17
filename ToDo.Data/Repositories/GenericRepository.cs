using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ToDo.Data.Entities;
using ToDo.Data.Entities.Base;


namespace ToDo.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }


        public async Task<IEnumerable<T>> GetAllAsync(string includeProperties)
        {
            IQueryable<T> query = _dbSet.Where(e => !e.IsDeleted).AsQueryable();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetManyByFilterAsync(Expression<Func<T, bool>> filter, string includeProperties)
        {
            IQueryable<T> set = _dbSet.Where(filter).AsQueryable();
            set = set.Where(e => !e.IsDeleted).AsQueryable();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (string includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    set = set.Include(includeProperty).Where(x => x.IsDeleted == false);

                }
            }

            return await set.ToListAsync();
        }

        public async Task<T?> GetByIDAsync(long id, string includeProperties)
        {
            IQueryable<T> query = _dbSet.Where(e => e.Id == id && !e.IsDeleted).AsQueryable();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<T?> GetOneByFilter(Expression<Func<T, bool>> filter, string includeProperties)
        {
            IQueryable<T> query = _dbSet.Where(filter).Where(e => !e.IsDeleted);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.Now;
            //entity.CreatedBy = _claimsProvider.GetUserName();
            var result = await _dbSet.AddAsync(entity);
            return result.Entity;
        }

        public Task<T> UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.Now;
            _dbSet.Update(entity);
            return Task.FromResult(entity);
        }

        public Task DeleteAsync(T entity, bool softDelete = true)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (softDelete)
            {
                entity.IsDeleted = true;
                _dbSet.Update(entity);
            }
            else
            {
                _dbSet.Remove(entity);
            }

            return Task.CompletedTask;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
