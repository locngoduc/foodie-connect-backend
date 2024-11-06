namespace foodie_connect_backend.Shared.Classes.Errors;

public abstract class AuthError
{
    private const string NotAuthenticatedCode = "NOT_AUTHENTICATED";
    private const string NotAuthorizedCode = "NOT_AUTHORIZED";
    public const string InvalidCredentialsCode = "INVALID_CREDENTIALS";
    
    public static AppError NotAuthenticated()
    {
        return new AppError(
            NotAuthenticatedCode,
            "Authentication is required to access this resource.");
    }

    public static AppError NotAuthorized()
    {
        return new AppError(
            NotAuthorizedCode,
            $"You are not authorized to access this resource.");
    }

    public static AppError InvalidCredentials()
    {
        return new AppError(InvalidCredentialsCode, "Invalid credentials.");
    }
}