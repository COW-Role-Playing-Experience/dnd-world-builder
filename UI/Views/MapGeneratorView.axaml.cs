

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using UI.ViewModels;
using System.Windows.Input;
using Avalonia;
using Avalonia.Media.Imaging;

namespace UI.Views;

public partial class MapGeneratorView : UserControl
{
    private Canvas _canvas;
    public MapGeneratorView()
    {
        InitializeComponent();
        DataContext = new MapGeneratorViewModel();
        initialiseThemesBox();


    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    //Initialise theme ComboBox
    private void initialiseThemesBox()
    {
        ComboBox themeBox = this.FindControl<ComboBox>("ThemesBox");
        (DataContext as MapGeneratorViewModel)?.makeThemeBoxes(themeBox);
    }


    private void GenerateSeed(object sender, RoutedEventArgs e)
    {
        TextBox SeedTextBox = this.FindControl<TextBox>("SeedTextBox");
        (DataContext as MapGeneratorViewModel)?.GenerateSeed(SeedTextBox);
    }

    private async void InitCanvas(object sender, VisualTreeAttachmentEventArgs e)
    {

    }

    private void SeedBoxWritten(object sender, TextChangedEventArgs e)
    {
        (DataContext as MapGeneratorViewModel)?.SeedBoxWritten(sender, e);
    }

    private void GenerateFromSeed(object? sender, RoutedEventArgs e)
    {
        var mapControl = this.FindControl<Canvas>("Map");

        if (mapControl != null) (DataContext as MapGeneratorViewModel)?.GenerateMap(mapControl);
    }
}