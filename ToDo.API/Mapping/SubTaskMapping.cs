using ToDo.API.Dtos.SubTaskDtos;
using ToDo.Data.Entities;

namespace ToDo.API.Mapping
{
    public static class SubTaskMappingExtensions
    {
        public static SubTask ToEntity(this CreateSubTaskRequestDto dto)
        {
            return new SubTask
            {
                ToDoId = dto.ToDoId,
                Title = dto.Title
            };
        }

        public static SubTaskResponseDto ToResponseDto(this SubTask subTask)
        {
            return new SubTaskResponseDto(
                subTask.Id,
                subTask.ToDoId,
                subTask.ToDo?.Title ?? string.Empty,
                subTask.Title,
                subTask.Status,
                subTask.CreatedAt,
                subTask.UpdatedAt
            );
        }

        public static void UpdateEntity(this UpdateSubTaskRequestDto dto, SubTask subTask)
        {
            subTask.Title = dto.Title;
            subTask.Status = dto.Status;
        }
    }
}
