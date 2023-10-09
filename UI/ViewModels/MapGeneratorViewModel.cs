
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
using UI.Classes;
using static System.Net.Mime.MediaTypeNames;
using Avalonia.Controls.ApplicationLifetimes;
using System.Diagnostics;
using Avalonia;

namespace UI.ViewModels;


public class MapGeneratorViewModel : ViewModelBase
{
    public void GenerateSeed(TextBox SeedTextBox)
    {
        Random random = new Random();
        int randomNumber = random.Next(1, 999999999);
        SeedTextBox.Text = randomNumber.ToString();
        MapHandler.MapSeed = randomNumber;
    }

    public void TextBoxWritten(object sender, TextChangedEventArgs e)
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
            switch (textBox.Name)
            {
                case "xSizeBox":
                    MapHandler.XSize = result;
                    break;
                case "ySizeBox":
                    MapHandler.YSize = result;
                    break;
                case "SeedTextBox":
                    MapHandler.MapSeed = result;
                    break;
            }
        }
    }

    public void SelectTheme(ComboBox themeBox, Button mapGenButton)
    {
        string selectedTheme = (String)themeBox.SelectedItem;
        selectedTheme = selectedTheme.ToLower();
        MapHandler.Theme = selectedTheme;
        mapGenButton.IsEnabled = true;
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

    public void ExportMap()
    {
        var documentsDirectory = Directory.GetCurrentDirectory();
        const string appFolderName = ".worldcrucible";
        const string mapsFolderName = "Maps";

        var appFolderPath = Path.Combine(documentsDirectory, appFolderName);
        var mapsFolderPath = Path.Combine(appFolderPath, mapsFolderName);

        if (!Directory.Exists(mapsFolderPath))
        {
            try
            {
                Directory.CreateDirectory(mapsFolderPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating directory: {ex.Message}");
            }
        }
        var newMapPath = Path.Combine(mapsFolderPath, "map-" + MapHandler.MapSeed + ".jpg");
        new FileRenderPipeline(MapHandler.map, 96, FileRenderPipeline.JpegEncoder(newMapPath, 90)).Render();
        Console.WriteLine("File saved to " + newMapPath);
    }

}