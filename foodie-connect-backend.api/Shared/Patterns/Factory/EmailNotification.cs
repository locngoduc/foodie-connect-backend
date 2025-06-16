using FluentEmail.Core;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;

namespace foodie_connect_backend.Shared.Patterns.Factory;

/// <summary>
/// Concrete email notification implementation for Factory Method pattern
/// </summary>
public class EmailNotification : INotification
{
    private readonly IFluentEmail _fluentEmail;

    public EmailNotification(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public string NotificationType => "Email";

    public async Task<Result<bool>> SendAsync(string recipient, string subject, string content)
    {
        try
        {
            var response = await _fluentEmail
                .To(recipient)
                .Subject(subject)
                .Body(content, true)
                .SendAsync();

            if (response.Successful)
            {
                return Result<bool>.Success(true);
            }

            var errorMessage = string.Join(", ", response.ErrorMessages);
            return Result<bool>.Failure(AppError.UnexpectedError($"Email sending failed: {errorMessage}"));
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(AppError.UnexpectedError($"Email notification error: {ex.Message}"));
        }
    }
} 