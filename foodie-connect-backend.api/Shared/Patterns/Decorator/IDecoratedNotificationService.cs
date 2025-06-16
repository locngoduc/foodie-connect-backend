using foodie_connect_backend.Shared.Classes;

namespace foodie_connect_backend.Shared.Patterns.Decorator;

/// <summary>
/// Specific interface for decorated notification service to avoid DI conflicts
/// </summary>
public interface IDecoratedNotificationService : ILoggableService
{
    // This interface inherits all methods from ILoggableService
    // and serves as a specific type for notification service injection
} 