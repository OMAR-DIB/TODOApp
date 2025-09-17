using ToDo.API.Dtos.ToDosDtos;
using ToDo.Data.Entities;

namespace ToDo.API.Mapping
{
    public static class ToDosMappingExtensions
    {
        public static ToDos ToEntity(this CreateToDosRequestDto dto, int userId)
        {
            return new ToDos
            {
                Title = dto.Title,
                Description = dto.Description,
                ToDoAt = dto.ToDoAt,
                UserId = userId
            };
        }

        public static ToDosResponseDto ToResponseDto(this ToDos todo)
        {
            return new ToDosResponseDto(
                todo.Id,
                todo.Title,
                todo.Description,
                todo.Status,
                todo.ToDoAt,
                todo.UserId,
                todo.User?.Username ?? string.Empty,
                todo.CreatedAt,
                todo.UpdatedAt,
                todo.SubTasks?.Select(st => new SubTaskResponseDto(
                    st.Id,
                    st.Title,
                    st.Status,
                    st.CreatedAt
                )).ToList()
            );
        }

        public static void UpdateEntity(this UpdateToDosRequestDto dto, ToDos todo)
        {
            todo.Title = dto.Title;
            todo.Description = dto.Description;
            todo.Status = dto.Status;
            todo.ToDoAt = dto.ToDoAt;
        }
    }
}
