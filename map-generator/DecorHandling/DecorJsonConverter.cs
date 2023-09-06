using System.Text.Json;
using System.Text.Json.Serialization;

namespace map_generator.DecorHandling;

public class DecorJsonConverter : JsonConverter<Decor>
{
    public override Decor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Decor(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, Decor value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}