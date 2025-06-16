using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;

namespace foodie_connect_backend.Shared.Patterns.Factory;

/// <summary>
/// Concrete push notification implementation for Factory Method pattern
/// This is a mock implementation for demonstration purposes
/// </summary>
public class PushNotification : INotification
{
    private readonly ILogger<PushNotification> _logger;

    public PushNotification(ILogger<PushNotification> logger)
    {
        _logger = logger;
    }

    public string NotificationType => "Push";

    public async Task<Result<bool>> SendAsync(string recipient, string subject, string content)
    {
        try
        {
            // Mock push notification logic - in real implementation, you would integrate with FCM/APNS
            _logger.LogInformation("Sending Push notification to {Recipient}: {Subject} - {Content}", recipient, subject, content);
            
            // Simulate async operation
            await Task.Delay(50);
            
            // Mock success response
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send push notification to {Recipient}", recipient);
            return Result<bool>.Failure(AppError.UnexpectedError($"Push notification error: {ex.Message}"));
        }
    }
} 