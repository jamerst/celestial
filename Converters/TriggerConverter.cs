using System.Text.Json;
using System.Text.Json.Serialization;

using Celestial.Triggers;

namespace Celestial.Converters;

public class TriggerConverter : JsonConverter<Trigger>
{
    public override Trigger? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Utf8JsonReader clone = reader;

        if (clone.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        string? type = null;
        StringComparison comparison = options.PropertyNameCaseInsensitive
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        while (type == null && clone.Read())
        {
            if (clone.TokenType == JsonTokenType.PropertyName)
            {
                if (string.Equals(clone.GetString(), nameof(Trigger.Type), comparison))
                {
                    clone.Read();

                    if (clone.TokenType == JsonTokenType.String)
                    {
                        type = clone.GetString();
                        break;
                    }
                    else
                    {
                        throw new JsonException($@"Expected ""{nameof(Trigger.Type)}"" to be a string, was {clone.TokenType}");
                    }
                }
            }
        }

        if (type == null)
        {
            throw new JsonException($@"Property ""{nameof(Trigger.Type)}"" not found");
        }

        return type switch
        {
            CelestialTrigger.TypeName => JsonSerializer.Deserialize<CelestialTrigger>(ref reader, options),
            TimeTrigger.TypeName => JsonSerializer.Deserialize<TimeTrigger>(ref reader, options),
            _ => throw new JsonException($@"Unknown trigger type ""{type}""")
        };
    }

    public override void Write(Utf8JsonWriter writer, Trigger value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}