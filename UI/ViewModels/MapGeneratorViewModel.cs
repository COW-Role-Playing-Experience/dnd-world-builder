
using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

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
}