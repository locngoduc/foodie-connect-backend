using System.Text.Json;
using System.Text.Json.Serialization;
using foodie_connect_backend.Extensions;

namespace foodie_connect_backend.Shared.Converters;

public class EnumOrStructJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return !typeToConvert.IsNullableType() && (typeToConvert.IsEnum || (Nullable.GetUnderlyingType(typeToConvert) != null &&
                                                                     typeToConvert.IsValueType && !typeToConvert.IsPrimitive));
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert.IsEnum)
        {
            var converterType = typeof(EnumOrStructJsonConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter) Activator.CreateInstance(converterType)!;
        }

        if (Nullable.GetUnderlyingType(typeToConvert) != null && typeToConvert.IsValueType &&
            !typeToConvert.IsPrimitive)
        {
            var converterType = typeof(StructJsonConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter) Activator.CreateInstance(converterType)!;
        }

        throw new NotSupportedException($"Type {typeToConvert} is not supported by this factory.");
    }
}

public class EnumOrStructJsonConverter<T> : JsonConverter<T> where T : struct, Enum
{
    private static readonly T InvalidValue = (T) Enum.ToObject(typeof(T), -1);

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            return (T) Enum.Parse(typeof(T), reader.GetString()!);
        }
        catch
        {
            return InvalidValue;
        }
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

public class StructJsonConverter<T> : JsonConverter<T> where T : struct
{
    private static readonly T InvalidValue = default;

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(ref reader, options);
        }
        catch
        {
            return InvalidValue;
        }
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
