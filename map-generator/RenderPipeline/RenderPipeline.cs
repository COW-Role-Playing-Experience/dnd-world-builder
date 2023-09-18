using map_generator.MapMaker;
using SixLabors.ImageSharp.Drawing.Processing;

namespace map_generator.RenderPipeline;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public class RenderPipeline
{
    private readonly Image<Rgba32> _canvas;

    public RenderPipeline(MapBuilder mb, int width, int height)
    {
        MapBuilder = mb;
        _canvas = new Image<Rgba32>(width, height);
    }

    public MapBuilder MapBuilder { set; private get; }

    public void Clear()
    {
        _canvas.Mutate(ftx => ftx.Fill(
            new Rgba32(0, 0, 0, 0),
            new Rectangle(0, 0, _canvas.Size.Width, _canvas.Size.Width))
        );
    }
}