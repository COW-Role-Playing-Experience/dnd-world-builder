using System.Collections.Concurrent;
using Avalonia.Media.Imaging;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Runtime.CompilerServices;
using map_generator.DecorHandling;
using map_generator.MapMaker;

namespace map_generator.RenderPipeline;

public abstract class AbstractRenderPipeline
{
    private Dictionary<RoomTile, Image<Rgba32>> _gridCache = new();
    private Dictionary<(Decor, float), Image<Rgba32>> _decorCache = new();


    protected Image<Rgba32> Canvas;
    protected MapBuilder? MapBuilder;

    protected AbstractRenderPipeline(MapBuilder? mb, int width, int height)
    {
        MapBuilder = mb;
        Canvas = new Image<Rgba32>(width, height);
    }

    /**
     * Clears the internal texture caches used by the pipeline.
     */
    public void ClearCache()
    {
        _gridCache.Clear();
        _decorCache.Clear();
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
            new Rgba32(255, 255, 255, 0),
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

    private void DrawGrid(float tlX, float tlY, float brX, float brY)
    {
        // Overflow is always tl
        int tileOriginX = (int)Math.Floor(tlX);
        int tileOriginY = (int)Math.Floor(tlY);

        int tileDestX = (int)Math.Ceiling(brX);
        int tileDestY = (int)Math.Ceiling(brY);

        float tileSizeX = Canvas.Size.Width / (brX - tlX);
        float tileSizeY = Canvas.Size.Height / (brY - tlY);

        float tileOffsetX = tileSizeX * (tlX - tileOriginX);
        float tileOffsetY = tileSizeY * (tlY - tileOriginY);

        RoomTile[,] tiles = MapBuilder!.getTiles();

        Canvas.Mutate(canvas =>
        {
            for (int x = tileOriginX; x < tileDestX - 1; x++)
            {
                for (int y = tileOriginY; y < tileDestY - 1; y++)
                {
                    // Skip if tile is out of bounds
                    if (
                        x < 0 || x >= tiles.GetLength(0)
                              || y < 0 || y >= tiles.GetLength(1)
                    ) continue;

                    RoomTile tile = tiles[x, y];

                    if (!_gridCache.TryGetValue(tile, out var texture))
                    {
                        texture = tile.getTexture().Clone();
                        //TODO: replace with proper ratio-aware scale factor, +1 works well when zoomed in but will distort when zoomed out
                        texture.Mutate(o => o.Resize((int)tileSizeX + 1, (int)tileSizeY + 1));
                        _gridCache[tile] = texture;
                    }

                    int x1 = Convert.ToInt32((x - tileOriginX) * tileSizeX - tileOffsetX);
                    int y1 = Convert.ToInt32((y - tileOriginY) * tileSizeY - tileOffsetY);

                    try
                    {
                        canvas.DrawImage(texture, new Point(x1, y1), 1f);
                    }
                    catch (ImageProcessingException ex)
                    {
                        // Skip tile if out of bounds. Should not occur often
                    }
                }
            }
        });
    }

    private void DrawDecor(float tlX, float tlY, float brX, float brY)
    {
        Canvas.Mutate(canvas =>
        {
            foreach (MetaTile tile in MapBuilder!.getMetaTiles())
            {
                if (
                    tile.XPos + tile.DecorGroup.Width < tlX
                    || tile.YPos + tile.DecorGroup.Height < tlY
                    || tile.XPos > brX
                    || tile.YPos > brY
                )
                {
                    continue;
                }

                float tileSizeX = Canvas.Size.Width / (brX - tlX);
                float tileSizeY = Canvas.Size.Height / (brY - tlY);

                float scaleX = tileSizeX / 200;
                float scaleY = tileSizeY / 200;

                foreach (var decor in tile.DecorGroup.Elements)
                {
                    float decorX = decor.XPos + tile.XPos;
                    float decorY = decor.YPos + tile.YPos;

                    float posX = Canvas.Size.Width - (brX - decorX) * tileSizeX;
                    float posY = Canvas.Size.Height - (brY - decorY) * tileSizeY;

                    var key = (decor.Decor, decor.Rotation);

                    if (!_decorCache.TryGetValue(key, out var texture))
                    {
                        texture = key.Decor.Texture.Clone();

                        texture.Mutate(px => px
                            .Resize((int)(scaleX * texture.Size.Width), (int)(scaleY * texture.Size.Height)));

                        texture.Mutate(px => px.Rotate(decor.Rotation));

                        _decorCache[key] = texture;
                    }

                    try
                    {
                        canvas.DrawImage(
                            texture,
                            new Point((int)(posX - texture.Width / 2f), (int)(posY - texture.Height / 2f)),
                            1f
                        );
                    }
                    catch (ImageProcessingException ex)
                    {
                        // Do nothing, out of bounds draws are to be expected.
                    }
                }
            }
        });
    }

    protected void Render(float tlX, float tlY, float brX, float brY)
    {
        if (MapBuilder == null) throw new NullReferenceException("Render pipeline failed due to unbound MapBuilder");
        this.Clear();

        this.DrawGrid(tlX, tlY, brX, brY);
        this.DrawDecor(tlX, tlY, brX, brY);

        //TODO: Re-enable baking!
        // Canvas.SaveAsPng($"{tlX}.png");
        this.Bake();
    }
}