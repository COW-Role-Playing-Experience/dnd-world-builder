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

    public MapBuilder MapBuilder { set; protected get; }

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

    protected abstract void Bake();

    protected void Render(float tlX, float tlY, float brX, float brY)
    {
        this.Clear();
        this.Bake();
    }
}