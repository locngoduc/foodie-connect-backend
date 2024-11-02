namespace foodie_connect_backend.Shared.Classes.Errors;

public sealed class AppError(string code, string message)
{
    public const string RecordNotFoundCode = "RecordNotFound";
    public const string ValidationErrorCode = "ValidationError";
    public const string ConflictErrorCode = "Conflict";
    public const string InvalidCredentialErrorCode = "InvalidCredential";
    public const string BadTokenCode = "BadToken";
    public const string InternalErrorCode = "InternalError";

    public static readonly AppError None = new(string.Empty, string.Empty);
    public string Code { get; private set; } = code;
    public string Message { get; private set; } = message;

    public static AppError RecordNotFound(string message)
    {
        return new AppError(RecordNotFoundCode, message);
    }

    public static AppError ValidationError(string message)
    {
        return new AppError(ValidationErrorCode, message);
    }

    public static AppError Conflict(string message)
    {
        return new AppError(ConflictErrorCode, message);
    }

    public static AppError InvalidCredential(string message)
    {
        return new AppError(InvalidCredentialErrorCode, message);
    }

    public static AppError BadToken(string message)
    {
        return new AppError(BadTokenCode, message);
    }

    public static AppError InternalError(string message)
    {
        return new AppError(InternalErrorCode, message);
    }
}