using ToDo.Data.Entities.@enum;

namespace ToDo.API.Dtos.SubTaskDtos
{
    public record CreateSubTaskRequestDto(
        int ToDoId,
        string Title
    );

    public record UpdateSubTaskRequestDto(
        int Id,
        string Title,
        TaskStatuss Status
    );

    public record SubTaskResponseDto(
        int Id,
        int ToDoId,
        string ToDoTitle,
        string Title,
        TaskStatuss Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
