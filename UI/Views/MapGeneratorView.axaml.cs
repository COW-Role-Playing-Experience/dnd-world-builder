using System;
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
using Avalonia.Controls.Primitives;

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

    private void ThemeSelected(object sender, SelectionChangedEventArgs e)
    {
        Button mapGenButton = this.FindControl<Button>("MapGenButton");
        ComboBox themeBox = this.FindControl<ComboBox>("ThemesBox");
        (DataContext as MapGeneratorViewModel)?.SelectTheme(themeBox, mapGenButton);
    }


    private void GenerateNewValues(object sender, RoutedEventArgs e)
    {
        var SeedTextBox = this.FindControl<TextBox>("SeedTextBox");
        var XSizeTextBox = this.FindControl<TextBox>("xSizeBox");
        var YSizeTextBox = this.FindControl<TextBox>("ySizeBox");
        (DataContext as MapGeneratorViewModel)?.GenerateNewValues(SeedTextBox, XSizeTextBox, YSizeTextBox);
    }

    private void ExportMap(object sender, RoutedEventArgs e)
    {
        (DataContext as MapGeneratorViewModel)?.ExportMap();
    }

    private void GenerateMap(object sender, RoutedEventArgs e)
    {
        Image? map = this.FindControl<Image>("Map");
        var SeedTextBox = this.FindControl<TextBox>("SeedTextBox");
        var XSizeTextBox = this.FindControl<TextBox>("xSizeBox");
        var YSizeTextBox = this.FindControl<TextBox>("ySizeBox");
        var exportButton = this.FindControl<Button>("ExportButton");
        var hostButton = this.FindControl<Button>("HostGameButton");
        (DataContext as MapGeneratorViewModel)?.GenerateMap(map, SeedTextBox, XSizeTextBox, YSizeTextBox, exportButton, hostButton);
    }

    private void TextBoxWritten(object sender, TextChangedEventArgs e)
    {
        (DataContext as MapGeneratorViewModel)?.TextBoxWritten(sender, e);
    }

    private void MapFillChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (sender is Slider slider)
        {
            (DataContext as MapGeneratorViewModel)?.setMapFill((float)slider.Value);
        }
    }
}