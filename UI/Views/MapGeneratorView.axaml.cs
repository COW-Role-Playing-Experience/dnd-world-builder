

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
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void GenerateSeed(object sender, RoutedEventArgs e)
    {
        TextBox SeedTextBox = this.FindControl<TextBox>("SeedTextBox");
        (DataContext as MapGeneratorViewModel)?.GenerateSeed(SeedTextBox);
    }

    private void SeedBoxWritten(object sender, TextChangedEventArgs e)
    {
        (DataContext as MapGeneratorViewModel)?.SeedBoxWritten(sender, e);
    }
}