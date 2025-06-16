using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using foodie_connect_backend.Shared.Dtos;
using foodie_connect_backend.Shared.Patterns.Decorator;

namespace foodie_connect_backend.Modules.Notifications;

[Route("v1/notifications")]
[ApiController]
[Produces("application/json")]
public class NotificationController : ControllerBase
{
    private readonly ILoggableService _notificationService;

    public NotificationController(ILoggableService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Send a single notification using Factory Method pattern
    /// Demonstrates both Factory Method and Decorator patterns
    /// </summary>
    /// <param name="request">Notification request</param>
    /// <returns>Result of the notification operation</returns>
    /// <response code="200">Notification sent successfully</response>
    /// <response code="400">Invalid notification type or request</response>
    /// <response code="401">User is not authenticated</response>
    [HttpPost("send")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult<GenericResponse>> SendNotification([FromBody] NotificationRequest request)
    {
        // The service is decorated with logging functionality (Decorator pattern)
        // and uses Factory Method pattern internally to create notifications
        var result = await _notificationService.ExecuteWithResultAsync(async () =>
        {
            var notificationService = _notificationService.DecoratedService as NotificationService 
                ?? throw new InvalidOperationException("Expected NotificationService");
            
            return await notificationService.SendNotificationAsync(
                request.Type, 
                request.Recipient, 
                request.Subject, 
                request.Content);
        }, "SendSingleNotification");

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(new GenericResponse { Message = "Notification sent successfully" });
    }

    /// <summary>
    /// Send multiple notifications of different types
    /// Demonstrates Factory Method pattern with multiple notification types
    /// </summary>
    /// <param name="requests">List of notification requests</param>
    /// <returns>Results of all notification operations</returns>
    /// <response code="200">Notifications processed</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">User is not authenticated</response>
    [HttpPost("send-multiple")]
    [ProducesResponseType(typeof(List<NotificationResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult<List<NotificationResult>>> SendMultipleNotifications([FromBody] List<NotificationRequest> requests)
    {
        var result = await _notificationService.ExecuteWithResultAsync(async () =>
        {
            var notificationService = _notificationService.DecoratedService as NotificationService 
                ?? throw new InvalidOperationException("Expected NotificationService");
            
            return await notificationService.SendMultipleNotificationsAsync(requests);
        }, "SendMultipleNotifications");

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get available notification types
    /// Shows the different types supported by the Factory Method pattern
    /// </summary>
    /// <returns>List of supported notification types</returns>
    /// <response code="200">List of notification types</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("types")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult<List<string>>> GetNotificationTypes()
    {
        var result = await _notificationService.ExecuteWithResultAsync(async () =>
        {
            var notificationService = _notificationService.DecoratedService as NotificationService 
                ?? throw new InvalidOperationException("Expected NotificationService");
            
            return await notificationService.GetAvailableNotificationTypesAsync();
        }, "GetNotificationTypes");

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }
} 