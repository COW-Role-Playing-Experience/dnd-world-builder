using Avalonia.Media.Imaging;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Runtime.CompilerServices;
using map_generator.MapMaker;

namespace map_generator.RenderPipeline;

public abstract class AbstractRenderPipeline
{
    protected Image<Rgba32> Canvas;

    protected AbstractRenderPipeline(MapBuilder mb, int width, int height)
    {
        MapBuilder = mb;
        Canvas = new Image<Rgba32>(width, height);
    }

    public MapBuilder MapBuilder { set; private get; }

    /**
     * Sets all pixels within the Image canvas to transparent.
     */
    public void Clear()
    {
        Canvas.Mutate(ftx => ftx.Fill(
            new Rgba32(0, 0, 0, 0),
            new Rectangle(0, 0, Canvas.Size.Width, Canvas.Size.Width))
        );
    }

    // TODO: Remove after testing.
    // Debug method used to generate a red -> blue colour shift
    private int _color = 0;
    public void Debug()
    {
        Canvas.Mutate(ftx => ftx.Fill(
            new Rgba32(_color, 0, 255 - _color, 255),
            new Rectangle(0, 0, Canvas.Size.Width, Canvas.Size.Width))
        );
        _color = (_color + 4) % 256;
    }

    protected abstract void Bake();

    protected void Render(float tlX, float tlY, float brX, float brY)
    {
        this.Clear();
        this.Debug(); //TODO: remove me!
        this.Bake();
    }
}