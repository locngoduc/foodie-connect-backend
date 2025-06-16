using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Patterns.Decorator;
using foodie_connect_backend.Shared.Patterns.Factory;

namespace foodie_connect_backend.Modules.Notifications;

/// <summary>
/// Service that demonstrates the use of Factory Method and Decorator patterns
/// </summary>
public class NotificationService : BaseLoggableService
{
    private readonly Dictionary<string, NotificationFactory> _notificationFactories;

    public NotificationService(
        EmailNotificationFactory emailFactory,
        SmsNotificationFactory smsFactory,
        PushNotificationFactory pushFactory)
    {
        _notificationFactories = new Dictionary<string, NotificationFactory>
        {
            { "email", emailFactory },
            { "sms", smsFactory },
            { "push", pushFactory }
        };
    }

    /// <summary>
    /// Sends a notification using the Factory Method pattern
    /// </summary>
    /// <param name="notificationType">Type of notification (email, sms, push)</param>
    /// <param name="recipient">Recipient of the notification</param>
    /// <param name="subject">Subject of the notification</param>
    /// <param name="content">Content of the notification</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<Result<bool>> SendNotificationAsync(
        string notificationType, 
        string recipient, 
        string subject, 
        string content)
    {
        return await ExecuteWithResultAsync(async () =>
        {
            if (!_notificationFactories.TryGetValue(notificationType.ToLower(), out var factory))
            {
                return Result<bool>.Failure(
                    new foodie_connect_backend.Shared.Classes.Errors.AppError(
                        "INVALID_NOTIFICATION_TYPE", 
                        $"Notification type '{notificationType}' is not supported"));
            }

            return await factory.SendNotificationAsync(recipient, subject, content);
        }, $"SendNotification-{notificationType}");
    }

    /// <summary>
    /// Sends multiple notifications of different types
    /// </summary>
    /// <param name="notifications">List of notification requests</param>
    /// <returns>Results of all notification attempts</returns>
    public async Task<Result<List<NotificationResult>>> SendMultipleNotificationsAsync(
        List<NotificationRequest> notifications)
    {
        return await ExecuteWithResultAsync(async () =>
        {
            var results = new List<NotificationResult>();

            foreach (var notification in notifications)
            {
                var result = await SendNotificationAsync(
                    notification.Type, 
                    notification.Recipient, 
                    notification.Subject, 
                    notification.Content);

                results.Add(new NotificationResult
                {
                    Type = notification.Type,
                    Recipient = notification.Recipient,
                    Success = result.IsSuccess,
                    ErrorMessage = result.IsFailure ? result.Error.Message : null
                });
            }

            return Result<List<NotificationResult>>.Success(results);
        }, "SendMultipleNotifications");
    }

    /// <summary>
    /// Gets available notification types
    /// </summary>
    /// <returns>List of supported notification types</returns>
    public async Task<Result<List<string>>> GetAvailableNotificationTypesAsync()
    {
        return await ExecuteWithResultAsync(async () =>
        {
            await Task.CompletedTask; // Simulate async operation
            return Result<List<string>>.Success(_notificationFactories.Keys.ToList());
        }, "GetAvailableNotificationTypes");
    }
}

/// <summary>
/// Request model for notifications
/// </summary>
public class NotificationRequest
{
    public string Type { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Result model for notification operations
/// </summary>
public class NotificationResult
{
    public string Type { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
} 