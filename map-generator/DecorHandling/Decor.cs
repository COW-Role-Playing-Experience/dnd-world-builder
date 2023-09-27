using map_generator.JsonLoading;

namespace map_generator.DecorHandling;

public record Decor(string ImageName)
{
    public Image<Rgba32> Texture => DataLoader.Textures[ImageName];

    public override int GetHashCode()
    {
        return ImageName.GetHashCode();
    }
}