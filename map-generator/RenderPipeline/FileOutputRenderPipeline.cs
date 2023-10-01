using map_generator.MapMaker;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace map_generator.RenderPipeline;

/**
 * Pipeline variant used to save to disk.
 */
public class FileRenderPipeline : AbstractRenderPipeline
{
    // Static encoder helper functions to simplify the creation of custom encoders.
    public static Action<Image<Rgba32>> PngEncoder(string path) => cv => cv.SaveAsPng(path);
    public static Action<Image<Rgba32>> JpegEncoder(string path, int quality) =>
        cv => cv.SaveAsJpeg(path, new JpegEncoder { Quality = quality });

    // Function used to save file.
    private Action<Image<Rgba32>> _saveFunction;

    public FileRenderPipeline(MapBuilder mb, int dpi, Action<Image<Rgba32>> saveFunction) :
        base(mb, mb.getTiles().GetLength(0) * dpi, mb.getTiles().GetLength(1) * dpi)
    {
        this._saveFunction = saveFunction;
    }

    /**
     * Render the given map to a file.
     */
    public void Render()
    {
        base.Render(0, 0, MapBuilder!.getTiles().GetLength(0), MapBuilder!.getTiles().GetLength(1));
    }

    protected override void Bake()
    {
        _saveFunction.Invoke(Canvas);
    }
}