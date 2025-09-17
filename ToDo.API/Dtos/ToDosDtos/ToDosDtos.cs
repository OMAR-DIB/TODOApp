using global::ToDo.Data.Entities.@enum;
using ToDo.Data.Entities.@enum;

namespace ToDo.API.Dtos.ToDosDtos
{
    public record CreateToDosRequestDto(
        string Title,
        string Description,
        DateOnly? ToDoAt
    );

    public record UpdateToDosRequestDto(
        int Id,
        string Title,
        string Description,
        TaskStatuss Status,
        DateOnly? ToDoAt
    );

    public record ToDosResponseDto(
        int Id,
        string Title,
        string Description,
        TaskStatuss Status,
        DateOnly? ToDoAt,
        int UserId,
        string Username,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        List<SubTaskResponseDto>? SubTasks = null
    );

    public record SubTaskResponseDto(
        int Id,
        string Title,
        TaskStatuss Status,
        DateTime CreatedAt
    );
}
