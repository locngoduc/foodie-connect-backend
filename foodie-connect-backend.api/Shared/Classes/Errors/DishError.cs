namespace foodie_connect_backend.Shared.Classes.Errors;

public abstract class DishError
{
    public const string DishNotFoundCode = "DISH_NOT_FOUND";
    public const string InvalidDishDataCode = "INVALID_DISH_DATA";
    public const string RestaurantNotFoundCode = "RESTAURANT_NOT_FOUND";
    public const string NotRestaurantOwnerCode = "NOT_RESTAURANT_OWNER";

    public static AppError DishNotFound(string id)
    {
        return new AppError(DishNotFoundCode, $"No dish with id '{id}' was found.");
    }

    public static AppError InvalidDishData(string message)
    {
        return new AppError(InvalidDishDataCode, $"Invalid dish data: {message}");
    }

    public static AppError RestaurantNotFound(string id)
    {
        return new AppError(RestaurantNotFoundCode, $"No restaurant with id '{id}' was found.");
    }

    public static AppError NotRestaurantOwner()
    {
        return new AppError(NotRestaurantOwnerCode, "You must be the restaurant owner to perform this action.");
    }
}