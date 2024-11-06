namespace foodie_connect_backend.Shared.Classes.Errors;

public abstract class UploadError
{
    public const string TypeNotAllowedCode = "TYPE_NOT_ALLOWED";
    public const string ExceedMaxSizeCode = "EXCEED_MAX_SIZE";

    public static AppError TypeNotAllowed(string inputType, List<string> allowedTypes)
    {
        return new AppError(
            TypeNotAllowedCode,
            $"The type '{inputType}' is not allowed. Allowed types are {string.Join(", ", allowedTypes)}.");
    }

    public static AppError ExceedMaxSize(double inputSize, double maxSize)
    {
        return new AppError(
            ExceedMaxSizeCode,
            $"The uploaded file ({inputSize / 8}MB) is too big. Max size is {maxSize / 8}MB.");
    }
}