namespace foodie_connect_backend.Shared.Classes;

public sealed class AreaError(string Code, string Message) : AppError(Code, Message)
{
    public static AppError AreaNotFound(string areaId)
    {
        return new AppError(
            "AreaNotFound",
            $"No area with the ID \"{areaId}\" was found");
    }

    public static AppError PermissionDenied(string permissionId)
    {
        return new AppError(
                "PermissionDenied",
                "You do not have permission to access this area"
            );
    }
}