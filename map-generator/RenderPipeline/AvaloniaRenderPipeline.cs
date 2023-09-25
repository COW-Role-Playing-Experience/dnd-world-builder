using System.Runtime.CompilerServices;
using Avalonia.Media.Imaging;
using map_generator.MapMaker;

namespace map_generator.RenderPipeline;

public class AvaloniaRenderPipeline : AbstractRenderPipeline
{
    private WriteableBitmap _writeableBitmap;

    public AvaloniaRenderPipeline(MapBuilder mapBuilder, WriteableBitmap writeableBitmap) :
        base(mapBuilder, (int)writeableBitmap.Size.Width, (int)writeableBitmap.Size.Height)
    {
        _writeableBitmap = writeableBitmap;
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
    public void Render(float x, float y, float zoom)
    {
        Render(0, 0, 0, 0);
    }
}