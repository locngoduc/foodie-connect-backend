namespace foodie_connect_backend.Shared.Classes.Errors;

public abstract class RestaurantServiceError
{
    public const string ServiceConflictCode = "SERVICE_ALREADY_EXISTS";
    public const string ServiceNotFoundCode = "SERVICE_NOT_FOUND";

    public static AppError ServiceConflict()
    {
        return new AppError(ServiceConflictCode, "Already added this service");
    }
    
    public static AppError ServiceNotFound()
    {
        return new AppError(ServiceNotFoundCode, "Service not found");
    }
}