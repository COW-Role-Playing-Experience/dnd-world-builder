using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Input.TextInput;
using Avalonia.Media.Imaging;
using map_generator.MapMaker;

namespace map_generator.RenderPipeline;

public class AvaloniaRenderPipeline : AbstractRenderPipeline
{
    private WriteableBitmap _writeableBitmap;
    private float _aspectRatio;
    private int _tileFactor;

    public AvaloniaRenderPipeline(MapBuilder mapBuilder, WriteableBitmap writeableBitmap) :
        base(mapBuilder, (int)writeableBitmap.Size.Width, (int)writeableBitmap.Size.Height)
    {
        _writeableBitmap = writeableBitmap;
        _aspectRatio = (float)writeableBitmap.Size.AspectRatio;
        _tileFactor = _aspectRatio < 1.0 ? mapBuilder.getTiles().GetLength(0) :
                mapBuilder.getTiles().GetLength(1);
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
     * Rebinds the RenderPipeline to a new WriteableBitmap.
     */
    public void Rebind(WriteableBitmap writeableBitmap)
    {
        _writeableBitmap = writeableBitmap;
        Canvas = new Image<Rgba32>((int)writeableBitmap.Size.Width, (int)writeableBitmap.Size.Height);
    }

    /**
     * Renders a new frame at the given location.
     */
    public void Render(float xPos, float yPos, float zoom)
    {
        int tilesX;
        int tilesY;

        int tileOriginX = (int)Math.Floor(xPos);
        int tileOriginY = (int)Math.Floor(yPos);

        int tileSize;
        if (_aspectRatio < 1.0)
        {
            // Get the amount of tiles over axis X to render
            // Use Convert RoundUp to handle halfway tiles
            tilesX = Convert.ToInt32(zoom * _tileFactor);
            tilesY = Convert.ToInt32(tilesX * _aspectRatio);

            // Image accuracy may be worth using a double
            tileSize = Convert.ToInt32(_writeableBitmap.Size.Width / tilesX);
        }
        else
        {
            tilesY = Convert.ToInt32(zoom * _tileFactor);
            tilesX = Convert.ToInt32(tilesY * _aspectRatio);

            tileSize = Convert.ToInt32(_writeableBitmap.Size.Height / tilesY);
        }

        for (int x = tileOriginX; x < tileOriginX + tilesX; x++)
        {
            for (int y = tileOriginY; y < tileOriginY + tilesY; y++)
            {
                // Use tilesize to determine tile image resize
                RoomTile tile = MapBuilder.getTiles()[x, y];
                Image<Rgba32>? texture = tile.getTexture()?.Clone();

                if (texture == null)
                {
                    continue;
                }

                texture.Mutate(o => o.Resize(tileSize, tileSize));

                var x1 = x;
                var y1 = y;

                Canvas.Mutate(o => o.DrawImage(texture, new Point(x1, y1), 1f));
            }
        }
    }
}