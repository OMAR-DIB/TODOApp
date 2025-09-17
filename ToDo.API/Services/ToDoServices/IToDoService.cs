using ToDo.API.Dtos.ToDosDtos;
using ToDo.Data.Entities.@enum;

namespace ToDo.API.Services.ToDoServices
{
    public interface IToDoService
    {
        Task<IEnumerable<ToDosResponseDto>> GetAllToDosAsync(int? userId = null);
        Task<IEnumerable<ToDosResponseDto>> GetToDosByStatusAsync(TaskStatuss status, int? userId = null);
        Task<IEnumerable<ToDosResponseDto>> GetUserToDosAsync(int userId);
        Task<ToDosResponseDto?> GetToDoByIdAsync(int id, int? userId = null);
        Task<ToDosResponseDto> CreateToDoAsync(CreateToDosRequestDto dto, int userId);
        Task<ToDosResponseDto> UpdateToDoAsync(UpdateToDosRequestDto dto, int userId);
        Task<bool> DeleteToDoAsync(int id, int userId);
        Task<bool> ToDoExistsAsync(int id, int? userId = null);
        Task<IEnumerable<ToDosResponseDto>> GetToDosForDateAsync(DateOnly date, int? userId = null);
    }
}
