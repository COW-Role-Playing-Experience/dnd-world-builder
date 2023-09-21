using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.Interactivity;
using UI.ViewModels;

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
}