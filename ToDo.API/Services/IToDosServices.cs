using ToDo.API.Dtos.RequestTask;
using ToDo.API.Dtos.Response;

namespace ToDo.API.Services
{
    public interface IToDosServices
    {
        Task<IEnumerable<ResponseTaskDto>> GetAll();
        Task<ResponseTaskDto?> GetTaskById(int id);
        Task<bool> DeleteTask(int id);
        Task<ResponseTaskDto> AddTask(RequestTaskDto task);
        Task<ResponseTaskDto> UpdateTask(int id, RequestTaskDto task);


    }
}
