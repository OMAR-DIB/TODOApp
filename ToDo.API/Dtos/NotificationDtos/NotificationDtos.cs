namespace ToDo.API.Dtos.NotificationDtos
{
    public record CreateNotificationRequestDto(
        int UserId,
        int ToDoId,
        string Message
    );

    public record UpdateNotificationRequestDto(
        int Id,
        string Message,
        bool IsRead
    );

    public record MarkAsReadRequestDto(int Id);

    public record NotificationResponseDto(
        int Id,
        int UserId,
        string Username,
        int ToDoId,
        string ToDoTitle,
        string Message,
        bool IsRead,
        DateTime CreatedAt
    );
}
