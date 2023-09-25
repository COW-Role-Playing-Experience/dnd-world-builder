

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using UI.ViewModels;
using System.Windows.Input;

namespace UI.Views;

public partial class MapGeneratorView : UserControl
{
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

    private void GenerateMap(object sender, RoutedEventArgs e)
    {
        (DataContext as MapGeneratorViewModel)?.GenerateMap();
    }

    private void SeedBoxWritten(object sender, TextChangedEventArgs e)
    {
        (DataContext as MapGeneratorViewModel)?.SeedBoxWritten(sender, e);
    }
}