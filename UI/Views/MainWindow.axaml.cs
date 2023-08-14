using Avalonia.Controls;
using Avalonia.ReactiveUI;
using UI.ViewModels;

namespace UI.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

    }
}
