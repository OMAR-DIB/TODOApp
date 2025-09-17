using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDo.API.ClaimProvider;
using ToDo.API.Dtos.NotificationDtos;
using ToDo.API.Services.NotificationServices;

namespace ToDo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IValidator<CreateNotificationRequestDto> _createValidator;
        private readonly IValidator<UpdateNotificationRequestDto> _updateValidator;
        private readonly IValidator<MarkAsReadRequestDto> _markAsReadValidator;
        private readonly IClaimsProvider _claimsProvider;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(
            INotificationService notificationService,
            IValidator<CreateNotificationRequestDto> createValidator,
            IValidator<UpdateNotificationRequestDto> updateValidator,
            IValidator<MarkAsReadRequestDto> markAsReadValidator,
            IClaimsProvider claimsProvider,
            ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _markAsReadValidator = markAsReadValidator;
            _claimsProvider = claimsProvider;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetUserNotifications([FromQuery] bool? isRead = null)
        {
            try
            {
                var userId = (int)_claimsProvider.GetUserID();
                var notifications = await _notificationService.GetUserNotificationsAsync(userId, isRead);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserNotifications");
                return StatusCode(500, "An error occurred while retrieving notifications");
            }
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            try
            {
                var userId = (int)_claimsProvider.GetUserID();
                var count = await _notificationService.GetUnreadCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUnreadCount");
                return StatusCode(500, "An error occurred while retrieving unread count");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationResponseDto>> GetNotification(int id)
        {
            try
            {
                var userId = (int)_claimsProvider.GetUserID();
                var notification = await _notificationService.GetNotificationByIdAsync(id, userId);
                if (notification == null)
                {
                    return NotFound($"Notification with ID {id} not found");
                }
                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetNotification for ID: {NotificationId}", id);
                return StatusCode(500, "An error occurred while retrieving the notification");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NotificationResponseDto>> CreateNotification(CreateNotificationRequestDto dto)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var notification = await _notificationService.CreateNotificationAsync(dto);
                return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateNotification");
                return StatusCode(500, "An error occurred while creating the notification");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<NotificationResponseDto>> UpdateNotification(int id, UpdateNotificationRequestDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest("ID mismatch between route and body");
                }

                var validationResult = await _updateValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var userId = (int)_claimsProvider.GetUserID();
                var notification = await _notificationService.UpdateNotificationAsync(dto, userId);
                return Ok(notification);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateNotification for ID: {NotificationId}", id);
                return StatusCode(500, "An error occurred while updating the notification");
            }
        }

        [HttpPatch("{id}/mark-as-read")]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            try
            {
                var dto = new MarkAsReadRequestDto(id);
                var validationResult = await _markAsReadValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var userId = (int)_claimsProvider.GetUserID();
                var success = await _notificationService.MarkAsReadAsync(id, userId);
                if (!success)
                {
                    return NotFound($"Notification with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MarkAsRead for ID: {NotificationId}", id);
                return StatusCode(500, "An error occurred while marking notification as read");
            }
        }

        [HttpPatch("mark-all-as-read")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = (int)_claimsProvider.GetUserID();
                await _notificationService.MarkAllAsReadAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MarkAllAsRead");
                return StatusCode(500, "An error occurred while marking all notifications as read");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNotification(int id)
        {
            try
            {
                var userId = (int)_claimsProvider.GetUserID();
                var success = await _notificationService.DeleteNotificationAsync(id, userId);
                if (!success)
                {
                    return NotFound($"Notification with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteNotification for ID: {NotificationId}", id);
                return StatusCode(500, "An error occurred while deleting the notification");
            }
        }
    }
}
