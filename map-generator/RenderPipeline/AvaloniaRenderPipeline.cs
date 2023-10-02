using System.Runtime.CompilerServices;
using Avalonia.Media.Imaging;
using map_generator.MapMaker;

namespace map_generator.RenderPipeline;

public class AvaloniaRenderPipeline : AbstractRenderPipeline
{
    private WriteableBitmap? _writeableBitmap;
    private int _bitmapWidth;
    private int _bitmapHeight;
    private float _prevZoom = Single.NaN;

    public AvaloniaRenderPipeline(MapBuilder? mapBuilder, WriteableBitmap? writeableBitmap) :
        base(mapBuilder, (int)(writeableBitmap?.Size.Width ?? 1), (int)(writeableBitmap?.Size.Height ?? 1))
    {
        _writeableBitmap = writeableBitmap;
        _bitmapWidth = (int)(writeableBitmap?.Size.Width ?? 1);
        _bitmapHeight = (int)(writeableBitmap?.Size.Height ?? 1);
    }

    /**
     * TODO: remove after pipeline completed/integrated!
     *
     * Debugging constructor which fake-binds a render context. Be sure not to Bake()!
     */
    public AvaloniaRenderPipeline(MapBuilder mapBuilder, int width, int height) :
        base(mapBuilder, width, height)
    {
        _bitmapWidth = width;
        _bitmapHeight = height;
    }

    /**
    * Bakes the Image canvas into the Avalonia WriteableBitmap.
    *
    * <summary>
    * <para>
    * This involves the use of unsafe C# code in order to copy the contents of the Image&lt;Rgba32>
    * into the the byte[] buffer of Avalonia's WriteableBitmap. As this has to be accessed via pointer,
    * it requires unsafe's pointer dereferencing. By using direct memory access, this function is able
    * to process at up to 3x the rate of similar code using Marshal.Copy().
    * </para>>
    *
    * <summary>
    * <para>
    * This method requires an Avalonia context to be initialised before it can be run.
    * </para>>
    * </summary>>
    * </summary>>
    */
    protected override unsafe void Bake()
    {
        if (_writeableBitmap == null)
        {
            throw new NullReferenceException("Render pipeline failed due to unbound WritableBitmap");
        }

        // Obtains the pointer from the WriteableBitmap
        using var buffer = _writeableBitmap.Lock();
        IntPtr ptr = buffer.Address;

        // Writes the image data directly into the address of the byte[] buffer.
        Canvas.CopyPixelDataTo(new Span<byte>(
            (void*)ptr,
            Canvas.Width * Canvas.Height * Unsafe.SizeOf<Rgba32>())
        );
    }

    /**
     * Bind a new WriteableBitmap to this pipeline.
     */
    public void RebindBitmap(WriteableBitmap writeableBitmap)
    {
        _writeableBitmap = writeableBitmap;
        Canvas = new Image<Rgba32>((int)writeableBitmap.Size.Width, (int)writeableBitmap.Size.Height);
        _bitmapWidth = (int)(writeableBitmap?.Size.Width ?? 1);
        _bitmapHeight = (int)(writeableBitmap?.Size.Height ?? 1);
    }

    /**
     * Converts a location on the WriteableBitmap into a position on the map.
     */
    public (double x, double y) ScreenToWorldspace(double x, double y, float zoom, (double x, double y) screenPosition)
    {
        (float tilesX, float tilesY) = CalculateConstraints(zoom);

        double tlX = x - tilesX / 2;
        double tlY = y - tilesY / 2;

        double widthRatio = screenPosition.x / _bitmapWidth;
        double heightRatio = screenPosition.y / _bitmapHeight;

        return (
            tlX + tilesX * widthRatio,
            tlY + tilesY * heightRatio
        );
    }

    /**
     * Converts a position on the map into a location on the WriteableBitmap
     */
    public (double x, double y) WorldToScreenspace(double x, double y, float zoom, (double x, double y) worldPosition)
    {
        (float tilesX, float tilesY) = CalculateConstraints(zoom);
        double tlX = x - tilesX / 2;
        double tlY = y - tilesY / 2;

        return (
            (worldPosition.x - tlX) * _bitmapWidth / tilesX,
            (worldPosition.y - tlY) * _bitmapHeight / tilesY
        );
    }

    /**
     * Returns the width of a single tile.
     */
    public float TileSize(float zoom)
    {
        // Note that this only uses the width constraint, and ignores the height. These values should be congruent regardless.
        return _bitmapWidth / CalculateConstraints(zoom).tilesX;
    }

    /**
     * Provides the size of the region of tiles to be rendered.
     */
    private (float tilesX, float tilesY) CalculateConstraints(float zoom)
    {
        if (MapBuilder == null)
        {
            throw new NullReferenceException("Failed to calculate constraints due to unbound MapBuilder");
        }

        RoomTile[,] tiles = MapBuilder.getTiles();

        int mapWidth = tiles.GetLength(0);
        int mapHeight = tiles.GetLength(1);

        float mapAspectRatio = mapWidth / (float)mapHeight;

        float tilesX;
        float tilesY;

        float aspectRatio = (float)_bitmapWidth / (float)_bitmapHeight;

        if (aspectRatio > mapAspectRatio)
        {
            // Get the amount of tiles over the axis X to render
            tilesY = 1 / zoom * mapHeight;
            tilesX = tilesY * aspectRatio;
        }
        else
        {
            tilesX = 1 / zoom * mapWidth;
            tilesY = tilesX / aspectRatio;
        }

        return (tilesX, tilesY);
    }

    /**
     * Renders a new frame at the given location.
     */
    public void Render(float x, float y, float zoom)
    {
        if (zoom != _prevZoom) base.ClearCache();
        _prevZoom = zoom;
        (float tilesX, float tilesY) = CalculateConstraints(zoom);
        base.Render(x - tilesX / 2, y - tilesY / 2, x + tilesX / 2, y + tilesY / 2);
    }
}