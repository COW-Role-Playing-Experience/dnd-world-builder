using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UI.Views;

public partial class DMView : UserControl
{
    public DMView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}