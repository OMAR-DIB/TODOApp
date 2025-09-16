using Microsoft.EntityFrameworkCore;
using ToDo.Data.Entities;

namespace ToDo.Data.Repositories.TaskRepo
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;
        public TaskRepository(AppDbContext context) {
            _context = context;
        }
        public async Task createTask(ToDos task)
        {
            await _context.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(ToDos task)
        {
             _context.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ToDos>> getAllTasks()
        {
           return await _context.Set<ToDos>().ToListAsync();
        }

        public async Task<ToDos?> GetById(int id)
        {
            return await _context.Set<ToDos>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateProduct(ToDos task)
        {
            _context.Set<ToDos>().Update(task);
            await _context.SaveChangesAsync();
        }
    }
}
