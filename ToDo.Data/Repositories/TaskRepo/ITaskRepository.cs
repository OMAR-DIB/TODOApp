using ToDo.Data.Entities;

namespace ToDo.Data.Repositories.TaskRepo
{
    public interface ITaskRepository
    {
        Task createTask(ToDos task);
        Task<IEnumerable<ToDos>> getAllTasks();
        Task UpdateProduct(ToDos task);
        Task DeleteProduct(ToDos task);
        Task<ToDos?> GetById(int id);
    }
}
