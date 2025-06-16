namespace foodie_connect_backend.Shared.Patterns.Factory;

/// <summary>
/// Abstract factory class implementing the Factory Method pattern for notifications
/// </summary>
public abstract class NotificationFactory
{
    /// <summary>
    /// Factory method to be implemented by concrete factories
    /// </summary>
    /// <returns>A concrete notification implementation</returns>
    public abstract INotification CreateNotification();

    /// <summary>
    /// Template method that uses the factory method
    /// </summary>
    /// <param name="recipient">Notification recipient</param>
    /// <param name="subject">Notification subject</param>
    /// <param name="content">Notification content</param>
    /// <returns>Result of the notification sending operation</returns>
    public async Task<foodie_connect_backend.Shared.Classes.Result<bool>> SendNotificationAsync(
        string recipient, 
        string subject, 
        string content)
    {
        var notification = CreateNotification();
        return await notification.SendAsync(recipient, subject, content);
    }
} 