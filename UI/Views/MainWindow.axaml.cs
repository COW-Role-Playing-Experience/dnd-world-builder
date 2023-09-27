using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using UI.ViewModels;

namespace UI.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
