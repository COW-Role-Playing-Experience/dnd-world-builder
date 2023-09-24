
using System;
using Avalonia.Controls;
using System.Windows.Input;
using System.Linq;
using System.IO;

namespace UI.ViewModels;


public class MapGeneratorViewModel : ViewModelBase
{
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