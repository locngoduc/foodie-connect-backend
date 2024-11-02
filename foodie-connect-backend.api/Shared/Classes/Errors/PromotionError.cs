namespace foodie_connect_backend.Shared.Classes.Errors;

public abstract class PromotionError
{
    public static AppError PromotionNotFound(string promotionId)
    {
        return new AppError(
            "PromotionNotFound",
            $"No promotion with the ID \"{promotionId}\" was found");
    }
}