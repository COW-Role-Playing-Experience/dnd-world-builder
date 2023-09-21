
using System;
using Avalonia.Controls;
using System.Windows.Input;
using System.Linq;

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
}