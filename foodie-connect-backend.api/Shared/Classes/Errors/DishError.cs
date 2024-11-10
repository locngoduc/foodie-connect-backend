namespace foodie_connect_backend.Shared.Classes.Errors;

public abstract class DishError
{
    public const string DishNotFoundCode = "DISH_NOT_FOUND";
    public const string InvalidDishDataCode = "INVALID_DISH_DATA";
    public const string RestaurantNotFoundCode = "RESTAURANT_NOT_FOUND";
    public const string NotRestaurantOwnerCode = "NOT_RESTAURANT_OWNER";
    public const string NameAlreadyExistsCode = "NAME_ALREADY_EXISTS";
    public const string AlreadyReviewedCode = "ALREADY_REVIEWED";
    public const string ReviewNotFoundCode = "REVIEW_NOT_FOUND";

    public static AppError DishNotFound()
    {
        return new AppError(DishNotFoundCode, $"This dish does not exist");
    }

    public static AppError InvalidDishData(string message)
    {
        return new AppError(InvalidDishDataCode, $"Invalid dish data: {message}");
    }

    public static AppError RestaurantNotFound()
    {
        return new AppError(RestaurantNotFoundCode, $"This restaurant does not exist");
    }

    public static AppError NotRestaurantOwner()
    {
        return new AppError(NotRestaurantOwnerCode, "You must be the restaurant owner to perform this action");
    }

    public static AppError NameAlreadyExists(string name)
    {
        return new AppError(NameAlreadyExistsCode, $"The name '{name}' already exists");
    }

    public static AppError AlreadyReviewed()
    {
        return new AppError(AlreadyReviewedCode, $"You already reviewed this dish");
    }

    public static AppError ReviewNotFound()
    {
        return new AppError(ReviewNotFoundCode, $"This review does not exist");
    }
}