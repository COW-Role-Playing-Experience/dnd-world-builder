
using System;
using Avalonia.Controls;
using System.Windows.Input;
using System.Linq;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using map_generator.JsonLoading;
using map_generator.MapMaker;
using map_generator.RenderPipeline;

namespace UI.ViewModels;


public class MapGeneratorViewModel : ViewModelBase
{
    // private ImageBrush ImageBrush

    private WriteableBitmap? _writeableBitmap;
    private AvaloniaRenderPipeline? _renderPipeline;
    public ImageBrush? ImageBrush { get; set; }

    private int MapSeed = new Random().Next(1, 999999999);
    public void GenerateSeed(TextBox SeedTextBox)
    {
        Random random = new Random();
        int randomNumber = random.Next(1, 999999999);
        SeedTextBox.Text = randomNumber.ToString();
    }

    public void SeedBoxWritten(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        string text = textBox.Text;
        if (!int.TryParse(text, out int result))
        {
            int caretIndex = textBox.CaretIndex;
            textBox.Text = string.Concat(text.Where(char.IsDigit));
            textBox.CaretIndex = Math.Max(0, caretIndex - 1);
        }
        else
        {
            MapSeed = result;
        }
    }

    public async Task GenerateMap(Canvas canvas)
    {
        //TODO this is where we implement the map generation
        // Random rng = new Random(MapSeed);
        // DataLoader.Random = rng;
        // DataLoader.Init();
        // Console.WriteLine("Hello, World!");
        // MapBuilder map = new MapBuilder(200, 40, rng, 0.8);
        // map.setTheme($"{DataLoader.RootPath}/data/dungeon-theme/").initRoom().fillGaps();
        // This should be refactored for optimization, for now I am putting it here for testing
        // if (_writeableBitmap == null)
        // {
        //     int width = (int)canvas.Bounds.Width;
        //     int height = (int)canvas.Bounds.Height;
        //     _writeableBitmap =
        //         new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Rgba8888, AlphaFormat.Unpremul);
        //     ImageBrush = new(_writeableBitmap);
        // }
        // _renderPipeline = new AvaloniaRenderPipeline(map, _writeableBitmap);
        // _renderPipeline.Render(10, 10, 0.1f);
        //
        // ImageBrush = new(_writeableBitmap);
        // canvas.Background = ImageBrush;
    }

    //Dynamically add Themes to the map generator view, based on what folders exist in Assets\Data
    public void makeThemeBoxes(ComboBox themesBox)
    {

        //Get array of subdirectories
        string directoryPrefix = Directory.GetCurrentDirectory().Split(new[] { "UI" }, StringSplitOptions.None)[0];
        string dataDirectory = Path.Combine(directoryPrefix, "Assets", "data");
        if (!Directory.Exists(dataDirectory)) return;
        string[] subdirectories = Directory.GetDirectories(dataDirectory);
        //Loop through each subdirectory
        foreach (string subdirectory in subdirectories)
        {
            if (subdirectory.Contains("-theme"))
            {
                //Process and add theme name to ComboBox
                string themeName = subdirectory.Split(new[] { "Assets" + Path.DirectorySeparatorChar + "data" + Path.DirectorySeparatorChar }, StringSplitOptions.None)[1];
                themeName = themeName.Split(new[] { "-" }, StringSplitOptions.None)[0];
                string themeCapitalized = char.ToUpper(themeName[0]) + themeName.Substring(1);
                themesBox.Items.Add(themeCapitalized);
            }
        }
    }

}