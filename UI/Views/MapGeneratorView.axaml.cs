

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using UI.ViewModels;
using System.Windows.Input;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using map_generator.JsonLoading;
using map_generator.RenderPipeline;
using UI.Classes;

namespace UI.Views;

public partial class MapGeneratorView : UserControl
{
    private Canvas _canvas;
    public MapGeneratorView()
    {
        InitializeComponent();
        DataContext = new MapGeneratorViewModel();
        initialiseThemesBox();
        MapHandler.RebindBitmap(new WriteableBitmap(
                new PixelSize(1920, 1080),
                new Vector(96, 96),
                Avalonia.Platform.PixelFormat.Rgba8888,
                AlphaFormat.Unpremul
                ));
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

    private void ThemeSelected(object sender, SelectionChangedEventArgs e){
        ComboBox themeBox = this.FindControl<ComboBox>("ThemesBox");
        (DataContext as MapGeneratorViewModel)?.SelectTheme(themeBox);
    }


    private void GenerateSeed(object sender, RoutedEventArgs e)
    {
        TextBox SeedTextBox = this.FindControl<TextBox>("SeedTextBox");
        (DataContext as MapGeneratorViewModel)?.GenerateSeed(SeedTextBox);
    }

    private void ExportMap(object sender, RoutedEventArgs e)
    {
        (DataContext as MapGeneratorViewModel)?.ExportMap();
    }

    private void GenerateMap(object sender, RoutedEventArgs e)
    {
        Image? map = this.FindControl<Image>("Map");
        MapHandler.GenerateMap(map);
    }

    private void SeedBoxWritten(object sender, TextChangedEventArgs e)
    {
        (DataContext as MapGeneratorViewModel)?.SeedBoxWritten(sender, e);
    }
}