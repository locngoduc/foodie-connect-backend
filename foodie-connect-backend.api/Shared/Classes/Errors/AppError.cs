namespace foodie_connect_backend.Shared.Classes.Errors;

public sealed class AppError(string code, string message)
{
    public const string InternalErrorCode = "INTERNAL_ERROR";
    public const string UnexpectedErrorCode = "UNEXPECTED_ERROR";
    public const string MissingRequiredFieldCode = "MISSING_REQUIRED_FIELD";
    
    public static readonly AppError None = new(string.Empty, string.Empty);
    public string Code { get; private set; } = code;
    public string Message { get; private set; } = message;

    public static AppError InternalError(string? message = null)
    {
        return new AppError(
            InternalErrorCode,
            $"An internal error has occurred. {message ?? string.Empty}");
    }

    public static AppError UnexpectedError(string message)
    {
        return new AppError(UnexpectedErrorCode, message);
    }

    public static AppError MissingRequiredField(string message)
    {
        return new AppError(MissingRequiredFieldCode, message);
    }
}