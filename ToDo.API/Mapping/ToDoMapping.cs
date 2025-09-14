using ToDo.API.Dtos.RequestTask;
using ToDo.API.Dtos.Response;
using ToDo.Data.Entities;

namespace ToDo.API.Mapping
{
    public static class ToDoMapping
    {
        public static ToDos ToEntity(this RequestTaskDto dto)
        {
            return new ToDos
            {
                Title = dto.Title,
                Description = dto.Description,
                ToDoAt = dto.ToDoAt
            };
        }

        public static ResponseTaskDto ToResponseDto(this ToDos entity)
        {
            return new ResponseTaskDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                ToDoAt = entity.ToDoAt ?? default, // safe fallback if null
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
