using System;
using System.Reactive.PlatformServices;
using Avalonia.Controls;
using ReactiveUI;

namespace UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;

    public MainWindowViewModel()
    {
        UI = new HomeScreenViewModel();
        _currentViewModel = UI;
    }

    public HomeScreenViewModel UI { get; }

    public ViewModelBase Current
    {
        get => _currentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    // View Switchers
    public void ReturnHome()
    {
        Current = new HomeScreenViewModel();
    }
    public void CreateView()
    {
        Current = new MapGeneratorViewModel();
    }
    public void DMView()
    {
        Current = new DMViewModel();
    }

    public void PlayerView()
    {
        Current = new PlayerViewModel();
    }


}