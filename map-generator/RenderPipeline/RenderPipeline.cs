using Avalonia.Media.Imaging;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Runtime.CompilerServices;
using map_generator.MapMaker;

namespace map_generator.RenderPipeline;

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
     * </summary>>
     */
    private unsafe void Bake(WriteableBitmap bm)
    {
        // Obtains the pointer from the WriteableBitmap
        using var buffer = bm.Lock();
        IntPtr ptr = buffer.Address;

        // Writes the image data directly into the address of the byte[] buffer.
        _canvas.CopyPixelDataTo(new Span<byte>(
            (void*)ptr,
            _canvas.Width * _canvas.Height * Unsafe.SizeOf<Rgba32>())
        );
    }

    /**
     * Renders a new frame to the provided WriteableBitmap.
     */
    public void Render(WriteableBitmap bm)
    {
        this.Clear();
        this.Bake(bm);
    }
}