namespace foodie_connect_backend.Shared.Patterns.Factory;

/// <summary>
/// Concrete factory for creating push notifications
/// </summary>
public class PushNotificationFactory : NotificationFactory
{
    private readonly ILogger<PushNotification> _logger;

    public PushNotificationFactory(ILogger<PushNotification> logger)
    {
        _logger = logger;
    }

    public override INotification CreateNotification()
    {
        return new PushNotification(_logger);
    }
} 