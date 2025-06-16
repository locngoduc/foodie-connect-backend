using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;

namespace foodie_connect_backend.Shared.Patterns.Factory;

/// <summary>
/// Concrete SMS notification implementation for Factory Method pattern
/// This is a mock implementation for demonstration purposes
/// </summary>
public class SmsNotification : INotification
{
    private readonly ILogger<SmsNotification> _logger;

    public SmsNotification(ILogger<SmsNotification> logger)
    {
        _logger = logger;
    }

    public string NotificationType => "SMS";

    public async Task<Result<bool>> SendAsync(string recipient, string subject, string content)
    {
        try
        {
            // Mock SMS sending logic - in real implementation, you would integrate with SMS provider
            _logger.LogInformation("Sending SMS to {Recipient}: {Subject} - {Content}", recipient, subject, content);
            
            // Simulate async operation
            await Task.Delay(100);
            
            // Mock success response
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS to {Recipient}", recipient);
            return Result<bool>.Failure(AppError.UnexpectedError($"SMS notification error: {ex.Message}"));
        }
    }
} 