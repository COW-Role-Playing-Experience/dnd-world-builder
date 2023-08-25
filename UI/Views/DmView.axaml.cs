using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using UI.ViewModels;

namespace UI.Views;

public partial class DmView : UserControl
{
    public DmView()
    {
        InitializeComponent();
        DataContext = new DmViewModel();

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}