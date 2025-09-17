using ToDo.API.Dtos.NotificationDtos;
using ToDo.API.Mapping;
using ToDo.Data.Entities;
using ToDo.Data.Repositories;

namespace ToDo.API.Services.NotificationServices
{
    public class NotificationService : INotificationService
    {
        private readonly IGenericRepository<Notification> _notificationRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<ToDos> _todoRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IGenericRepository<Notification> notificationRepository,
            IGenericRepository<User> userRepository,
            IGenericRepository<ToDos> todoRepository,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _todoRepository = todoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetUserNotificationsAsync(int userId, bool? isRead = null)
        {
            try
            {
                IEnumerable<Notification> notifications;

                if (isRead.HasValue)
                {
                    notifications = await _notificationRepository.GetManyByFilterAsync(
                        n => n.UserId == userId && n.IsRead == isRead.Value,
                        "User,ToDo"
                    );
                }
                else
                {
                    notifications = await _notificationRepository.GetManyByFilterAsync(
                        n => n.UserId == userId,
                        "User,ToDo"
                    );
                }

                return notifications.Select(n => n.ToResponseDto()).OrderByDescending(n => n.CreatedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving notifications for user: {UserId}", userId);
                throw new InvalidOperationException($"Failed to retrieve notifications for user: {userId}", ex);
            }
        }

        public async Task<NotificationResponseDto?> GetNotificationByIdAsync(int id, int userId)
        {
            try
            {
                var notification = await _notificationRepository.GetOneByFilter(
                    n => n.Id == id && n.UserId == userId,
                    "User,ToDo"
                );

                return notification?.ToResponseDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving notification with ID: {NotificationId} for user: {UserId}", id, userId);
                throw new InvalidOperationException($"Failed to retrieve notification with ID: {id}", ex);
            }
        }

        public async Task<NotificationResponseDto> CreateNotificationAsync(CreateNotificationRequestDto dto)
        {
            try
            {
                // Verify user and todo exist
                var userExists = await _userRepository.AnyAsync(u => u.Id == dto.UserId && !u.IsDeleted);
                if (!userExists)
                {
                    throw new InvalidOperationException($"User with ID {dto.UserId} not found");
                }

                var todoExists = await _todoRepository.AnyAsync(t => t.Id == dto.ToDoId && !t.IsDeleted);
                if (!todoExists)
                {
                    throw new InvalidOperationException($"ToDo with ID {dto.ToDoId} not found");
                }

                var notification = dto.ToEntity();
                var createdNotification = await _notificationRepository.AddAsync(notification);
                await _notificationRepository.SaveChangesAsync();

                // Fetch with navigation properties
                var result = await _notificationRepository.GetByIDAsync(createdNotification.Id, "User,ToDo");

                _logger.LogInformation("Notification created successfully with ID: {NotificationId}", createdNotification.Id);
                return result!.ToResponseDto();
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                _logger.LogError(ex, "Error occurred while creating notification");
                throw new InvalidOperationException("Failed to create notification", ex);
            }
        }

        public async Task<NotificationResponseDto> UpdateNotificationAsync(UpdateNotificationRequestDto dto, int userId)
        {
            try
            {
                var existingNotification = await _notificationRepository.GetOneByFilter(
                    n => n.Id == dto.Id && n.UserId == userId,
                    "User,ToDo"
                );

                if (existingNotification == null)
                {
                    throw new InvalidOperationException($"Notification with ID {dto.Id} not found or you don't have permission to update it");
                }

                dto.UpdateEntity(existingNotification);
                await _notificationRepository.UpdateAsync(existingNotification);
                await _notificationRepository.SaveChangesAsync();

                _logger.LogInformation("Notification updated successfully with ID: {NotificationId} for user: {UserId}", dto.Id, userId);
                return existingNotification.ToResponseDto();
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                _logger.LogError(ex, "Error occurred while updating notification with ID: {NotificationId} for user: {UserId}", dto.Id, userId);
                throw new InvalidOperationException($"Failed to update notification with ID: {dto.Id}", ex);
            }
        }

        public async Task<bool> MarkAsReadAsync(int id, int userId)
        {
            try
            {
                var notification = await _notificationRepository.GetOneByFilter(
                    n => n.Id == id && n.UserId == userId,
                    ""
                );

                if (notification == null)
                {
                    return false;
                }

                notification.IsRead = true;
                await _notificationRepository.UpdateAsync(notification);
                await _notificationRepository.SaveChangesAsync();

                _logger.LogInformation("Notification marked as read with ID: {NotificationId} for user: {UserId}", id, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while marking notification as read with ID: {NotificationId} for user: {UserId}", id, userId);
                throw new InvalidOperationException($"Failed to mark notification as read with ID: {id}", ex);
            }
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            try
            {
                var notifications = await _notificationRepository.GetManyByFilterAsync(
                    n => n.UserId == userId && !n.IsRead,
                    ""
                );

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                    await _notificationRepository.UpdateAsync(notification);
                }

                await _notificationRepository.SaveChangesAsync();

                _logger.LogInformation("All notifications marked as read for user: {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while marking all notifications as read for user: {UserId}", userId);
                throw new InvalidOperationException($"Failed to mark all notifications as read for user: {userId}", ex);
            }
        }

        public async Task<bool> DeleteNotificationAsync(int id, int userId)
        {
            try
            {
                var notification = await _notificationRepository.GetOneByFilter(
                    n => n.Id == id && n.UserId == userId,
                    ""
                );

                if (notification == null)
                {
                    return false;
                }

                await _notificationRepository.DeleteAsync(notification, softDelete: true);
                await _notificationRepository.SaveChangesAsync();

                _logger.LogInformation("Notification deleted successfully with ID: {NotificationId} for user: {UserId}", id, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting notification with ID: {NotificationId} for user: {UserId}", id, userId);
                throw new InvalidOperationException($"Failed to delete notification with ID: {id}", ex);
            }
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            try
            {
                var unreadNotifications = await _notificationRepository.GetManyByFilterAsync(
                    n => n.UserId == userId && !n.IsRead,
                    ""
                );

                return unreadNotifications.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting unread count for user: {UserId}", userId);
                throw new InvalidOperationException($"Failed to get unread count for user: {userId}", ex);
            }
        }
    }
}
