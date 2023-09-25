using Avalonia.Media.Imaging;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Runtime.CompilerServices;
using map_generator.MapMaker;

namespace map_generator.RenderPipeline;

public abstract class AbstractRenderPipeline
{
    protected Image<Rgba32> Canvas;
    protected MapBuilder? MapBuilder;

    protected AbstractRenderPipeline(MapBuilder? mb, int width, int height)
    {
        MapBuilder = mb;
        Canvas = new Image<Rgba32>(width, height);
    }

    /**
     * Bind a new MapBuilder to this pipeline.
     */
    public virtual void RebindBuilder(MapBuilder mb)
    {
        MapBuilder = mb;
    }

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

        // Overflow is always tl
        int tileOriginX = (int)Math.Floor(tlX);
        int tileOriginY = (int)Math.Floor(tlY);


        int tileSize;

        if (brX - tlX < brY - tlY)
        {
            tileSize = Convert.ToInt32(Canvas.Size.Width / (brX - tlX));
        }
        else
        {
            tileSize = Convert.ToInt32(Canvas.Size.Height / (brY - tlY));
        }

        int tileOffsetX = Convert.ToInt32(tileSize * (tlX - tileOriginX));
        int tileOffsetY = Convert.ToInt32(tileSize * (tlY - tileOriginY));

        for (int x = tileOriginX; x < tileOriginX + brX; x++)
        {
            for (int y = tileOriginY; y < tileOriginY + brY; y++)
            {
                RoomTile tile = MapBuilder!.getTiles()[x, y];
                Image<Rgba32>? texture = tile.getTexture()?.Clone();

                if (texture == null)
                {
                    continue;
                }

                texture.Mutate(o => o.Resize(tileSize, tileSize));

                int x1 = (x - tileOriginX) * tileSize - tileOffsetX;
                int y1 = (y - tileOriginY) * tileSize - tileOffsetY;

                Canvas.Mutate(o => o.DrawImage(texture, new Point(x1, y1), 1f));
            }
        }

        this.Bake();
    }
}