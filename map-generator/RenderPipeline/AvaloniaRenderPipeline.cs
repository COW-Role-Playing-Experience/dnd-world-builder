using System.Runtime.CompilerServices;
using Avalonia.Media.Imaging;
using map_generator.MapMaker;

namespace map_generator.RenderPipeline;

public class AvaloniaRenderPipeline : AbstractRenderPipeline
{
    private WriteableBitmap? _writeableBitmap;
    private float _aspectRatio;

    public AvaloniaRenderPipeline(MapBuilder? mapBuilder, WriteableBitmap? writeableBitmap) :
        base(mapBuilder, (int)(writeableBitmap?.Size.Width ?? 0), (int)(writeableBitmap?.Size.Height ?? 0))
    {
        _writeableBitmap = writeableBitmap;
        _aspectRatio = (float)(writeableBitmap?.Size.AspectRatio ?? 1.0f);
    }

    /**
     * TODO: remove after pipeline completed/integrated!
     *
     * Debugging constructor which fake-binds a render context. Be sure not to Bake()!
     */
    public AvaloniaRenderPipeline(MapBuilder mapBuilder, int width, int height) :
        base(mapBuilder, width, height)
    {
        _aspectRatio = width / (float)height;
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
        _aspectRatio = (float)writeableBitmap.Size.AspectRatio;
    }

    public override void RebindBuilder(MapBuilder mb)
    {
        base.RebindBuilder(mb);
        _aspectRatio = (float)(_writeableBitmap?.Size.AspectRatio ?? 1.0f);
    }

    /**
     * Renders a new frame at the given location.
     */
    public void Render(float x, float y, float zoom)
    {
        if (MapBuilder == null)
        {
            throw new NullReferenceException("Render pipeline failed due to unbound MapBuilder");
        }

        RoomTile[,] tiles = MapBuilder.getTiles();

        int mapWidth = tiles.GetLength(0);
        int mapHeight = tiles.GetLength(1);

        float mapAspectRatio = mapWidth / (float)mapHeight;

        float tilesX;
        float tilesY;



        if (_aspectRatio > mapAspectRatio)
        {
            // Get the amount of tiles over the axis X to render
            tilesY = 1 / zoom * mapHeight;
            tilesX = tilesY * _aspectRatio;
        }
        else
        {
            tilesX = 1 / zoom * mapWidth;
            tilesY = tilesX / _aspectRatio;
        }

        base.Render(x - tilesX / 2, y - tilesY / 2, x + tilesX / 2, y + tilesY / 2);
    }
}