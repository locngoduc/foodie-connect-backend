namespace foodie_connect_backend.Shared.Classes;

public sealed record AppError(string Code, string Message)
{
    public static readonly string RecordNotFoundCode = "RecordNotFound";
    public static readonly string ValidationErrorCode = "ValidationError";
    public static readonly string ConflictErrorCode = "Conflict";
    public static readonly string InvalidCredentialErrorCode = "InvalidCredential";
    public static readonly string BadTokenCode = "BadToken";
    
    public static readonly AppError None = new(string.Empty, string.Empty);
    
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
}