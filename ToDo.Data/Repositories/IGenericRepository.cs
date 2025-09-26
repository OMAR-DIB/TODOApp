
using ToDo.Data.Entities.Base;
using System.Linq.Expressions;
using ToDo.Data.Entities;

namespace ToDo.Data.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync(string includeProperties);
        Task<IEnumerable<T>> GetManyByFilterAsync(Expression<Func<T, bool>> filter, string includeProperties);
        Task<T?> GetByIDAsync(long id, string includeProperties);
        Task<T?> GetOneByFilter(Expression<Func<T, bool>> filter, string includeProperties = "");
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity, bool softDelete = true);
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
        Task<int> SaveChangesAsync();
        //Task<Role?> GetOneByFilter(Expression<Func<Role, bool>> predicate, string includeProperties = "");
    }
}
