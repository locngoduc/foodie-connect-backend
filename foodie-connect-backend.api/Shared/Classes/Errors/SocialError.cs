namespace foodie_connect_backend.Shared.Classes.Errors;

public abstract class SocialError
{
    public const string RestaurantNotFoundCode = "RESTAURANT_NOT_FOUND";
    public const string SocialAlreadyExistCode = "SOCIAL_ALREADY_EXISTS";
    public const string SocialDoesNotExistCode = "SOCIAL_NOT_FOUND";

    public static AppError RestaurantNotFound(string id)
    {
        return new AppError(RestaurantNotFoundCode, $"Restaurant with id \"{id}\" not found");
    }

    public static AppError SocialAlreadyExists(string type)
    {
        return new AppError(SocialAlreadyExistCode, $"Social platform with type \"{type}\" already exists for this restaurant.");
    }

    public static AppError SocialDoesNotExist(string id)
    {
        return new AppError(SocialDoesNotExistCode, $"Social platform with id \"{id}\" does not exist.");
    }
}