using FluentEmail.Core;
using foodie_connect_backend.Modules.Notifications;
using foodie_connect_backend.Modules.Payments;
using foodie_connect_backend.Shared.Patterns.Adapter;
using foodie_connect_backend.Shared.Patterns.Decorator;
using foodie_connect_backend.Shared.Patterns.Factory;

namespace foodie_connect_backend.Extensions.DI;

/// <summary>
/// Extension methods for registering custom design patterns in DI container
/// </summary>
public static class RegisterDesignPatterns
{
    /// <summary>
    /// Registers Factory Method, Decorator, and Adapter patterns in the DI container
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void AddDesignPatterns(this IServiceCollection services)
    {
        // Register Factory Method Pattern components
        RegisterFactoryMethodPattern(services);
        
        // Register Adapter Pattern components
        RegisterAdapterPattern(services);
        
        // Register Decorator Pattern components (after other services)
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

    private static void RegisterAdapterPattern(IServiceCollection services)
    {
        // Register legacy payment services (Adaptees)
        services.AddScoped<LegacyStripeService>();
        services.AddScoped<LegacyPayPalService>();
        services.AddScoped<LegacySquareService>();

        // Register payment gateway adapters
        services.AddScoped<StripePaymentAdapter>(provider =>
        {
            var stripeService = provider.GetRequiredService<LegacyStripeService>();
            return new StripePaymentAdapter(stripeService);
        });

        services.AddScoped<PayPalPaymentAdapter>(provider =>
        {
            var payPalService = provider.GetRequiredService<LegacyPayPalService>();
            return new PayPalPaymentAdapter(payPalService);
        });

        services.AddScoped<SquarePaymentAdapter>(provider =>
        {
            var squareService = provider.GetRequiredService<LegacySquareService>();
            return new SquarePaymentAdapter(squareService);
        });

        // Register the payment service that uses adapters
        services.AddScoped<PaymentService>();
    }

    private static void RegisterDecoratorPattern(IServiceCollection services)
    {
        // Register the base loggable service
        services.AddScoped<BaseLoggableService>();

        // Register decorated payment service for PaymentController
        services.AddScoped<ILoggableService>(provider =>
        {
            var paymentService = provider.GetRequiredService<PaymentService>();
            var logger = provider.GetRequiredService<ILogger<LoggingServiceDecorator>>();
            
            return new LoggingServiceDecorator(paymentService, logger);
        });

        // Register decorated notification service for NotificationController
        services.AddScoped<IDecoratedNotificationService>(provider =>
        {
            var notificationService = provider.GetRequiredService<NotificationService>();
            var logger = provider.GetRequiredService<ILogger<LoggingServiceDecorator>>();
            
            var decorator = new LoggingServiceDecorator(notificationService, logger);
            return new DecoratedNotificationService(decorator);
        });
    }
} 