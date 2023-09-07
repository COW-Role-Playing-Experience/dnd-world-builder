using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace map_generator.ImageSharpTest;

public class ImageTest
{
    public static readonly DirectoryInfo RootDirectory =
        new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent!.Parent!.Parent!.Parent!;

    public static void Demo()
    {
        // Create a 200x200 black image
        using (var image = new Image<Rgba32>(200, 200))
        {
            // Fill the image with black color
            image.Mutate(ctx => ctx.BackgroundColor(Color.Black));

            // Load the image you want to insert
            using (var overlayImage = Image.Load($"{RootDirectory}/Assets/Images/Large_Flagstone_A_04.jpg"))
            {
                // Resize the overlay image to fit within the 200x200 canvas
                overlayImage.Mutate(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(200, 200),
                    Mode = ResizeMode.Max
                }));

                // Overlay the image on top of the black canvas
                image.Mutate(ctx => ctx.DrawImage(overlayImage, new Point(0, 0), 1.0f));

                // Save the resulting image as a PNG
                image.Save("output.png", new PngEncoder());
            }
        }
    }
}