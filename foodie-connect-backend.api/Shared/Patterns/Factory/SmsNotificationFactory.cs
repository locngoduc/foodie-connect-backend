namespace foodie_connect_backend.Shared.Patterns.Factory;

/// <summary>
/// Concrete factory for creating SMS notifications
/// </summary>
public class SmsNotificationFactory : NotificationFactory
{
    private readonly ILogger<SmsNotification> _logger;

    public SmsNotificationFactory(ILogger<SmsNotification> logger)
    {
        _logger = logger;
    }

    public override INotification CreateNotification()
    {
        return new SmsNotification(_logger);
    }
} 