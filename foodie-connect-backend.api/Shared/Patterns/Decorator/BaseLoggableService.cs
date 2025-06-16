using foodie_connect_backend.Shared.Classes;

namespace foodie_connect_backend.Shared.Patterns.Decorator;

/// <summary>
/// Base implementation of ILoggableService - the concrete component in Decorator pattern
/// </summary>
public class BaseLoggableService : ILoggableService
{
    public virtual async Task<Result<T>> ExecuteWithResultAsync<T>(Func<Task<Result<T>>> operation, string operationName)
    {
        // Base implementation just executes the operation without any decoration
        return await operation();
    }

    public virtual async Task ExecuteAsync(Func<Task> operation, string operationName)
    {
        // Base implementation just executes the operation without any decoration
        await operation();
    }
} 