using ToDo.API.Dtos.SubTaskDtos;

namespace ToDo.API.Services.SubTaskServices
{
    public interface ISubTaskService
    {
        Task<IEnumerable<SubTaskResponseDto>> GetSubTasksByToDoAsync(int todoId, int userId);
        Task<SubTaskResponseDto?> GetSubTaskByIdAsync(int id, int userId);
        Task<SubTaskResponseDto> CreateSubTaskAsync(CreateSubTaskRequestDto dto, int userId);
        Task<SubTaskResponseDto> UpdateSubTaskAsync(UpdateSubTaskRequestDto dto, int userId);
        Task<bool> DeleteSubTaskAsync(int id, int userId);
        Task<bool> SubTaskExistsAsync(int id, int userId);
    }
}
