using foodie_connect_backend.Shared.Classes;

namespace foodie_connect_backend.Shared.Patterns.Decorator;

/// <summary>
/// Interface for services that can be decorated with logging functionality
/// This represents the component interface in the Decorator pattern
/// </summary>
public interface ILoggableService
{
    Task<Result<T>> ExecuteWithResultAsync<T>(Func<Task<Result<T>>> operation, string operationName);
    Task ExecuteAsync(Func<Task> operation, string operationName);
} 