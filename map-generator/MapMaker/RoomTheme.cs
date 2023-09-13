
using map_generator.DecorHandling;
using map_generator.JsonLoading;

namespace map_generator.MapMaker;

public record RoomTheme
{
    public required int id { get; init; }
    public required int minWidth { get; init; }
    public required int maxWidth { get; init; }
    public required int minHeight { get; init; }
    public required int maxHeight { get; init; }
    public required int[] connectorIds { get; init; }
    public required int minConnectors { get; init; }
    public required int maxConnectors { get; init; }

    public required string floorTexture { private get; init; }

    public required string decorGroup { private get; init; }

    // These are getters rather than properties, as properties should be side-effect free

    public Image<Rgba32> GetFloorTexture() => DataLoader.Textures[floorTexture];

    public DecorGroup GetDecorGroup() => DataLoader.DecorGroups[decorGroup];
}