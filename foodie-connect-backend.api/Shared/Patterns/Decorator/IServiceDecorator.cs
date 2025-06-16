namespace foodie_connect_backend.Shared.Patterns.Decorator;

/// <summary>
/// Base interface for service decorators implementing the Decorator pattern
/// </summary>
/// <typeparam name="T">The service type to be decorated</typeparam>
public interface IServiceDecorator<T> where T : class
{
    T DecoratedService { get; }
} 