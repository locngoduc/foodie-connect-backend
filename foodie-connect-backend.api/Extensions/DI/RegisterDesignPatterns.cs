using FluentEmail.Core;
using foodie_connect_backend.Modules.Notifications;
using foodie_connect_backend.Shared.Patterns.Decorator;
using foodie_connect_backend.Shared.Patterns.Factory;

namespace foodie_connect_backend.Extensions.DI;

/// <summary>
/// Extension methods for registering custom design patterns in DI container
/// </summary>
public static class RegisterDesignPatterns
{
    /// <summary>
    /// Registers Factory Method and Decorator patterns in the DI container
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void AddDesignPatterns(this IServiceCollection services)
    {
        // Register Factory Method Pattern components
        RegisterFactoryMethodPattern(services);
        
        // Register Decorator Pattern components
        RegisterDecoratorPattern(services);
    }

    private static void RegisterFactoryMethodPattern(IServiceCollection services)
    {
        // Register concrete notification factories
        services.AddScoped<EmailNotificationFactory>(provider =>
        {
            var fluentEmail = provider.GetRequiredService<IFluentEmail>();
            return new EmailNotificationFactory(fluentEmail);
        });

        services.AddScoped<SmsNotificationFactory>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<SmsNotification>>();
            return new SmsNotificationFactory(logger);
        });

        services.AddScoped<PushNotificationFactory>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<PushNotification>>();
            return new PushNotificationFactory(logger);
        });

        // Register the main notification service that uses the factories
        services.AddScoped<NotificationService>();
    }

    private static void RegisterDecoratorPattern(IServiceCollection services)
    {
        // Register the base loggable service
        services.AddScoped<BaseLoggableService>();

        // Register the decorated service using the Decorator pattern
        services.AddScoped<ILoggableService>(provider =>
        {
            var notificationService = provider.GetRequiredService<NotificationService>();
            var logger = provider.GetRequiredService<ILogger<LoggingServiceDecorator>>();
            
            // Wrap the notification service with logging decorator
            return new LoggingServiceDecorator(notificationService, logger);
        });
    }
} 