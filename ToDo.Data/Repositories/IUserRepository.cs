using ToDo.Data.Entities;

namespace ToDo.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task AddUserAsync(User user);
        Task SaveChangesAsync();
    }
}
