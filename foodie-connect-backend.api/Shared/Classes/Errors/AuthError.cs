namespace foodie_connect_backend.Shared.Classes.Errors;

public abstract class AuthError
{
    private const string NotAuthenticatedCode = "Not authenticated";
    private const string NotAuthorizedCode = "Not authorized";
    
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
}