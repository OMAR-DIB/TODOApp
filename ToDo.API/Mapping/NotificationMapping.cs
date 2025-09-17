using ToDo.API.Dtos.NotificationDtos;
using ToDo.Data.Entities;

namespace ToDo.API.Mapping
{
    public static class NotificationMappingExtensions
    {
        public static Notification ToEntity(this CreateNotificationRequestDto dto)
        {
            return new Notification
            {
                UserId = dto.UserId,
                ToDoId = dto.ToDoId,
                Message = dto.Message
            };
        }

        public static NotificationResponseDto ToResponseDto(this Notification notification)
        {
            return new NotificationResponseDto(
                notification.Id,
                notification.UserId,
                notification.User?.Username ?? string.Empty,
                notification.ToDoId,
                notification.ToDo?.Title ?? string.Empty,
                notification.Message,
                notification.IsRead,
                notification.CreatedAt
            );
        }

        public static void UpdateEntity(this UpdateNotificationRequestDto dto, Notification notification)
        {
            notification.Message = dto.Message;
            notification.IsRead = dto.IsRead;
        }
    }
}
