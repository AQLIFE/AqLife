using System.Text.Json;
using System.Text.Json.Serialization;

namespace AqLife.Web.Json;

public sealed class StrictEnumJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum &&
               !typeToConvert.IsDefined(typeof(FlagsAttribute), inherit: false);
    }

    public override JsonConverter CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var converterType = typeof(StrictEnumJsonConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

internal sealed class StrictEnumJsonConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    public override TEnum Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        object rawValue;

        try
        {
            rawValue = Enum.GetUnderlyingType(typeof(TEnum)) switch
            {
                var type when type == typeof(ulong) =>
                    reader.GetUInt64(),

                var type when type == typeof(uint) =>
                    checked((uint)reader.GetInt64()),

                var type when type == typeof(ushort) =>
                    checked((ushort)reader.GetInt64()),

                var type when type == typeof(byte) =>
                    checked((byte)reader.GetInt64()),

                _ =>
                    reader.GetInt64()
            };
        }
        catch (Exception ex) when (ex is FormatException or InvalidOperationException or OverflowException or JsonException)
        {
            throw new JsonException($"Invalid value for enum {typeof(TEnum).Name}.", ex);
        }

        if (!Enum.IsDefined(typeof(TEnum), rawValue))
        {
            throw new JsonException(
                $"Value '{rawValue}' is not defined for enum {typeof(TEnum).Name}.");
        }

        return (TEnum)Enum.ToObject(typeof(TEnum), rawValue);
    }

    public override void Write(
        Utf8JsonWriter writer,
        TEnum value,
        JsonSerializerOptions options)
    {
        writer.WriteNumberValue(Convert.ToInt64(value));
    }
}
