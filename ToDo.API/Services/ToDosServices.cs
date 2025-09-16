using ToDo.API.Dtos.RequestTask;
using ToDo.API.Dtos.Response;
using ToDo.API.Mapping;
using ToDo.Data.Entities;
using ToDo.Data.Repositories.Task;

namespace ToDo.API.Services
{
    public class ToDosServices : IToDosServices
    {
        
        
            private readonly ITaskRepository _repo;

            public ToDosServices(ITaskRepository repo)
            {
                _repo = repo;
            }

            public async Task<ResponseTaskDto> AddTask(RequestTaskDto task)
            {
                var entity = task.ToEntity();
                await _repo.createTask(entity);
                return entity.ToResponseDto();
            }

            public async Task<bool> DeleteTask(int id)
            {
                var task = await _repo.GetById(id);
                if (task == null) return false;

                await _repo.DeleteProduct(task);
                return true;
            }

            public async Task<IEnumerable<ResponseTaskDto>> GetAll()
            {
                var tasks = await _repo.getAllTasks();
                return tasks.Select(t => t.ToResponseDto());
            }

            public async Task<ResponseTaskDto?> GetTaskById(int id)
            {
                var task = await _repo.GetById(id);
                return task?.ToResponseDto();
            }

            public Task<ResponseTaskDto> UpdateTask(int id, RequestTaskDto task)
            {
                throw new NotImplementedException();
            }
        }
    }
