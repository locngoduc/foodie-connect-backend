namespace foodie_connect_backend.Shared.Classes.Errors;

public abstract class UserError
{
    public const string UserNotFoundCode = "USER_NOT_FOUND";
    public const string DuplicateUsernameCode = "USERNAME_ALREADY_EXISTS";
    public const string DuplicateEmailCode = "EMAIL_ALREADY_EXISTS";
    public const string PasswordMismatchCode = "PASSWORD_MISMATCH";
    public const string PasswordNotValidCode = "PASSWORD_NOT_VALID";

    public static AppError UserNotFound(string id)
    {
        return new AppError(UserNotFoundCode, $"User with id '{id}' not found");
    }

    public static AppError DuplicateUsername(string username)
    {
        return new AppError(DuplicateUsernameCode, $"The username '{username}' is already taken");
    }

    public static AppError DuplicateEmail(string email)
    {
        return new AppError(DuplicateEmailCode, $"The email '{email}' is already taken");
    }

    public static AppError PasswordMismatch()
    {
        return new AppError(PasswordMismatchCode, $"The provided password does not match the current password of the user");
    }

    public static AppError PasswordNotValid(string reason)
    {
        return new AppError(PasswordNotValidCode, reason);
    }
}