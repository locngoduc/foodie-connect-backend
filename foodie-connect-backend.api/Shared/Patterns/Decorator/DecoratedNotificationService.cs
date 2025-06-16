using foodie_connect_backend.Shared.Classes;

namespace foodie_connect_backend.Shared.Patterns.Decorator;

/// <summary>
/// Concrete wrapper for decorated notification service
/// </summary>
public class DecoratedNotificationService : IDecoratedNotificationService
{
    private readonly LoggingServiceDecorator _decorator;

    public DecoratedNotificationService(LoggingServiceDecorator decorator)
    {
        _decorator = decorator;
    }

    public object DecoratedService => _decorator.DecoratedService;

    public Task<Result<T>> ExecuteWithResultAsync<T>(Func<Task<Result<T>>> operation, string operationName)
    {
        return _decorator.ExecuteWithResultAsync(operation, operationName);
    }

    public Task ExecuteAsync(Func<Task> operation, string operationName)
    {
        return _decorator.ExecuteAsync(operation, operationName);
    }
} 