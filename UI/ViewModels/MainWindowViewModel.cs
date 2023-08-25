using ReactiveUI;

namespace UI.ViewModels;

/// <summary>
/// ViewModel for the main window of the application.
/// </summary>

public class MainWindowViewModel : ViewModelBase
{
    // Field to store the currently active ViewModel.
    private ViewModelBase _currentViewModel;

    // Initializes a new instance of the MainWindowViewModel class.
    public MainWindowViewModel()
    {
        Ui = new HomeScreenViewModel();
        _currentViewModel = Ui;
    }
    //Getter for instance.
    private HomeScreenViewModel Ui { get; }

    // Gets or sets the currently active ViewModel.
    public ViewModelBase Current
    {
        get => _currentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    // View Switchers.
    public void ReturnHome()
    {
        Current = new HomeScreenViewModel();
    }
    public void CreateView()
    {
        Current = new MapGeneratorViewModel();
    }
    public void DmView()
    {
        Current = new DmViewModel();
    }

    public void PlayerView()
    {
        Current = new PlayerViewModel();
    }


}