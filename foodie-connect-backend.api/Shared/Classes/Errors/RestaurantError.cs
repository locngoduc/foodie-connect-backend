namespace foodie_connect_backend.Shared.Classes.Errors;

public abstract class RestaurantError
{
    public const string RestaurantNotExistCode = "RESTAURANT_NOT_EXIST";
    public const string RestaurantUploadPartialErrorCode = "RESTAURANT_UPLOAD_PARTIAL";
    public const string IncorrectCoordinatesCode = "INCORRECT_COORDINATES";
    public const string ImageNotExistCode = "IMAGE_NOT_EXIST";

    public static AppError RestaurantNotExist(string id)
    {
        return new AppError(
            RestaurantNotExistCode,
            $"The restaurant with id \"{id}\" does not exist.");
    }

    public static AppError IncorrectCoordinates()
    {
        return new AppError(
            IncorrectCoordinatesCode,
            "These coordinates are invalid.");
    }

    public static AppError ImageNotExist(string id)
    {
        return new AppError(
            ImageNotExistCode,
            $"The image with id \"{id}\" does not exist.");
    }

    public static AppError RestaurantUploadPartialError()
    {
        return new AppError(
            RestaurantUploadPartialErrorCode,
            "Some images were unable to be uploaded.");
    }
}