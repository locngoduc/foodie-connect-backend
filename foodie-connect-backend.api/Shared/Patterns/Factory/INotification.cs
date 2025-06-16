using foodie_connect_backend.Shared.Classes;

namespace foodie_connect_backend.Shared.Patterns.Factory;

/// <summary>
/// Base interface for all notification types in the Factory Method pattern
/// </summary>
public interface INotification
{
    Task<Result<bool>> SendAsync(string recipient, string subject, string content);
    string NotificationType { get; }
} 