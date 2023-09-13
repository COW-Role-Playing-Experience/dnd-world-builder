namespace map_generator.JsonLoading;

/**
 * Delegate candidate which lazy-loads and supplies bitmap textures.
 */
public class LazyTextureProvider
{
    public LazyTextureProvider(string filePath)
    {
        _filePath = filePath;
    }

    private readonly string _filePath;
    private Image<Rgba32>? _image;
    public Image<Rgba32> Texture() => _image ??= Image.Load<Rgba32>(_filePath);
    public bool IsLoaded => _image != null;
}