using ToDo.API.Dtos.NotificationDtos;

namespace ToDo.API.Services.NotificationServices
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationResponseDto>> GetUserNotificationsAsync(int userId, bool? isRead = null);
        Task<NotificationResponseDto?> GetNotificationByIdAsync(int id, int userId);
        Task<NotificationResponseDto> CreateNotificationAsync(CreateNotificationRequestDto dto);
        Task<NotificationResponseDto> UpdateNotificationAsync(UpdateNotificationRequestDto dto, int userId);
        Task<bool> MarkAsReadAsync(int id, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteNotificationAsync(int id, int userId);
        Task<int> GetUnreadCountAsync(int userId);
    }
}
