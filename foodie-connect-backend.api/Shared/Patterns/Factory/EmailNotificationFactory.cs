using FluentEmail.Core;

namespace foodie_connect_backend.Shared.Patterns.Factory;

/// <summary>
/// Concrete factory for creating email notifications
/// </summary>
public class EmailNotificationFactory : NotificationFactory
{
    private readonly IFluentEmail _fluentEmail;

    public EmailNotificationFactory(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public override INotification CreateNotification()
    {
        return new EmailNotification(_fluentEmail);
    }
} 