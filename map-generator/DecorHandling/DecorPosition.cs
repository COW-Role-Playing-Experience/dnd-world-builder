using System.Text.Json.Serialization;

namespace map_generator.DecorHandling;

/**A data class for storing a Decor and its position/rotation*/
public record DecorPosition(
    [property: JsonPropertyName("name"), JsonConverter(typeof(DecorJsonConverter))] Decor Decor,
    [property: JsonPropertyName("x")] float XPos,
    [property: JsonPropertyName("y")] float YPos,
    float Rotation
);