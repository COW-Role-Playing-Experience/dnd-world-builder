
using System;
using Avalonia.Controls;
using System.Windows.Input;
using System.Linq;
using System.IO;
using map_generator.JsonLoading;
using map_generator.RenderPipeline;
using map_generator.MapMaker;
using Avalonia.Media.Imaging;
using Avalonia;
using System.Drawing.Imaging;
using Avalonia.Media;
using Avalonia.Platform;

namespace UI.ViewModels;


public class MapGeneratorViewModel : ViewModelBase
{
    private int MapSeed = new Random().Next(1, 999999999);
    public static AvaloniaRenderPipeline pipeline;
    public static WriteableBitmap buffer;
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

    public void initMapGen()
    {
        buffer = new WriteableBitmap(
            new PixelSize(1920, 1080),
            new Vector(96, 96),
            Avalonia.Platform.PixelFormat.Rgba8888,
            AlphaFormat.Unpremul
            );
        pipeline.RebindBitmap(buffer); //pipeline should be given the buffer
    }

    public void GenerateMap(Image mapImage)
    {
        Random rng = new Random(MapSeed);
        DataLoader.Random = rng;
        int xSize = 200;
        int ySize = 40;
        MapBuilder map = new MapBuilder(xSize, ySize, rng, 0.8);
        map.setTheme($"{DataLoader.RootPath}/data/dungeon-theme/").initRoom().fillGaps();
        pipeline.RebindBuilder(map); //bind the finished map to the renderer
        pipeline.Render(xSize / 2.0f, ySize / 2.0f, 1); //call once with the default to update bitmap
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