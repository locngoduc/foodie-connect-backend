namespace foodie_connect_backend.Shared.Classes.Errors;

public abstract class RestaurantError
{
    public const string RestaurantNotExistCode = "RESTAURANT_NOT_EXIST";
    public const string RestaurantUploadPartialErrorCode = "RESTAURANT_UPLOAD_PARTIAL";
    public const string IncorrectCoordinatesCode = "INCORRECT_COORDINATES";
    public const string ImageNotExistCode = "IMAGE_NOT_EXIST";
    public const string DishCategoryAlreadyExistCode = "DISH_CATEGORY_ALREADY_EXIST";
    public const string DishCategoryNotExistCode = "DISH_CATEGORY_NOT_EXIST";
    public const string NotOwnerCode = "NOT_OWNER";
    public const string DuplicateNameCode = "DUPLICATE_NAME";
    public const string UnsupportedQueryCode = "UNSUPPORTED_QUERY";
    public const string AlreadyReviewedCode = "ALREADY_REVIEWED";
    public const string ReviewNotExistCode = "REVIEW_NOT_EXIST";

    public static AppError RestaurantNotExist()
    {
        return new AppError(
            RestaurantNotExistCode,
            $"The restaurant does not exist.");
    }

    public static AppError IncorrectCoordinates()
    {
        return new AppError(
            IncorrectCoordinatesCode,
            "These coordinates are invalid. Expecting coordinates in format: 'longitude,latitude'");
    }

    public static AppError ImageNotExist(string id)
    {
        return new AppError(
            ImageNotExistCode,
            $"The image with id '{id}' does not exist.");
    }

    public static AppError RestaurantUploadPartialError()
    {
        return new AppError(
            RestaurantUploadPartialErrorCode,
            "Some images were unable to be uploaded.");
    }

    public static AppError DishCategoryAlreadyExist(string categoryName)
    {
        return new AppError(
            DishCategoryAlreadyExistCode,
            $"The category name '{categoryName}' already exist.");
    }

    public static AppError DishCategoryNotExist(string categoryName)
    {
        return new AppError(
            DishCategoryNotExistCode,
            $"The category name '{categoryName}' does not exist.");
    }

    public static AppError NotOwner()
    {
        return new AppError(
            NotOwnerCode,
            $"You are not the owner of this restaurant");
    }

    public static AppError DuplicateName(string restaurantName)
    {
        return new AppError(
            DuplicateNameCode,
            $"The restaurant name '{restaurantName}' already exist.");
    }

    public static AppError UnsupportedQuery()
    {
        return new AppError(
            UnsupportedQueryCode,
            "At least one query parameter is required.");
    }
    
    public static AppError AlreadyReviewed()
    {
        return new AppError(
            AlreadyReviewedCode,
            "You have already reviewed this restaurant.");
    }
    
    public static AppError ReviewNotExist()
    {
        return new AppError(
            ReviewNotExistCode,
            "The review does not exist.");
    }
}