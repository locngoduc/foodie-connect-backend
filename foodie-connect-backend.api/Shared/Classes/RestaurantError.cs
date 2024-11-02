namespace foodie_connect_backend.Shared.Classes;

public sealed class RestaurantError(string Code, string Message) : AppError(Code, Message)
{
    public static AppError PermissionDenied(string permissionId)
    {
        return new AppError(
            PermissionDeniedErrorCode,
            "You do not have permission to access this restaurant."
        );
    }
}