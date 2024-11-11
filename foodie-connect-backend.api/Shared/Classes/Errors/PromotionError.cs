namespace foodie_connect_backend.Shared.Classes.Errors;

public abstract class PromotionError
{
    public const string PromotionNotFoundCode = "PROMOTION_NOT_FOUND";
    public const string PromotionDishNotFoundCode = "PROMOTION_DISH_NOT_FOUND";
    public static AppError PromotionNotFound()
    {
        return new AppError(
            PromotionNotFoundCode,
            $"No promotion with the specified ID was found.");
    }
    
    public static AppError PromotionDishNotFound()
    {
        return new AppError(
            PromotionDishNotFoundCode,
            $"No dish with the specified ID was found in the restaurant for the promotion.");
    }
}