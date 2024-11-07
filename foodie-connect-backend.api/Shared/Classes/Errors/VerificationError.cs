namespace foodie_connect_backend.Shared.Classes.Errors;

public abstract class VerificationError
{
    public const string UserNotFoundCode = "USER_NOT_FOUND";
    public const string EmailAlreadyConfirmedCode = "EMAIL_ALREADY_CONFIRMED";
    public const string EmailVerificationTokenInvalidCode = "EMAIL_TOKEN_INVALID";

    public static AppError UserNotFound(string id)
    {
        return new AppError(UserNotFoundCode, $"User with id '{id}' not found");
    }

    public static AppError EmailAlreadyConfirmed()
    {
        return new AppError(EmailAlreadyConfirmedCode, "Email already confirmed");
    }

    public static AppError EmailVerificationTokenInvalid()
    {
        return new AppError(EmailVerificationTokenInvalidCode, "Email verification token invalid");
    }
}