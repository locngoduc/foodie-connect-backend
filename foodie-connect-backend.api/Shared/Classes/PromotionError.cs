namespace foodie_connect_backend.Shared.Classes;

public sealed class PromotionError(string Code, string Message) : AppError(Code, Message)
{
    public static AppError PromotionNotFound(string promotionId)
    {
        return new AppError(
            "PromotionNotFound",
            $"No promotion with the ID \"{promotionId}\" was found");
    }
}