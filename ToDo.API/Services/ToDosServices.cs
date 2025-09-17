using ToDo.API.Dtos.RequestTask;
using ToDo.API.Dtos.Response;
using ToDo.API.Services;
using ToDo.Data.Entities;
using ToDo.Data.Repositories; // ensure this namespace exists and contains IRepository<T>

public class ToDosServices : IToDosServices
{
    private readonly IGenericRepository<ToDos> _todoRepository;

    public ToDosServices(IGenericRepository<ToDos> todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<IEnumerable<ToDos>> GetAllAsync()
    {
        return await _todoRepository.GetAllAsync(includeProperties: "SubTasks,Notifications,User");
    }

    public async Task<ToDos?> GetByIdAsync(int id)
    {
        return await _todoRepository.GetByIDAsync(id, includeProperties: "SubTasks,Notifications,User");
    }

    public async Task<ToDos> CreateAsync(ToDos entity)
    {
        var result = await _todoRepository.AddAsync(entity);
        await _todoRepository.SaveChangesAsync();
        return result;
    }

    public async Task UpdateAsync(ToDos entity)
    {
        await _todoRepository.UpdateAsync(entity);
        await _todoRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(ToDos entity, bool softDelete = true)
    {
        await _todoRepository.DeleteAsync(entity, softDelete);
        await _todoRepository.SaveChangesAsync();
    }

    public Task<IEnumerable<ResponseTaskDto>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<ResponseTaskDto?> GetTaskById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteTask(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseTaskDto> AddTask(RequestTaskDto task)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseTaskDto> UpdateTask(int id, RequestTaskDto task)
    {
        throw new NotImplementedException();
    }
}
