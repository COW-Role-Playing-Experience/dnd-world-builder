

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using UI.ViewModels;
using System.Windows.Input;
using map_generator.JsonLoading;
using map_generator.RenderPipeline;

namespace UI.Views;

public partial class MapGeneratorView : UserControl
{
    public MapGeneratorView()
    {
        InitializeComponent();
        DataContext = new MapGeneratorViewModel();
        initialiseThemesBox();
        initialiseMapGen();
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


    private void initialiseMapGen()
    {
        (DataContext as MapGeneratorViewModel)?.initMapGen();
    }



    private void GenerateSeed(object sender, RoutedEventArgs e)
    {
        TextBox SeedTextBox = this.FindControl<TextBox>("SeedTextBox");
        (DataContext as MapGeneratorViewModel)?.GenerateSeed(SeedTextBox);
    }

    private void GenerateMap(object sender, RoutedEventArgs e)
    {
        Image map = this.FindControl<Image>("Map");
        (DataContext as MapGeneratorViewModel)?.GenerateMap(map);
    }

    private void SeedBoxWritten(object sender, TextChangedEventArgs e)
    {
        (DataContext as MapGeneratorViewModel)?.SeedBoxWritten(sender, e);
    }
}