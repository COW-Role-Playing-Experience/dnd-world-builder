using Avalonia.Controls;

namespace UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _contentViewModel;
    public MainWindowViewModel()
    {
    }

    public ViewModelBase ContentViewModel
    {
        get => _contentViewModel;



    }
}