namespace foodie_connect_backend.Shared.Classes.Errors;

public abstract class DishError
{
    public const string DishNotFoundCode = "DISH_NOT_FOUND";

    public static AppError DishNotFound(string id)
    {
        return new AppError(DishNotFoundCode, $"No dish with id \"{id}\" was found.");
    }
}