using map_generator.JsonLoading;

namespace map_generator.DecorHandling;

public record Decor(string ImageName)
{
    public readonly Image<Rgba32> Texture = DataLoader.Textures[ImageName];
}