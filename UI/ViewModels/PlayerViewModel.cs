namespace UI.ViewModels;
using System.Collections.ObjectModel;

using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ReactiveUI;
using Path = System.IO.Path;
public class PlayerViewModel : ViewModelBase
{

    // Private fields for UI state and token count.
    private bool _isUiVisible = true;
    private bool _isAddVisible;
    private double _uiButtonOpacity = 1.0;
    private int _tokenCount = 0;
    private int ObservableTokenCount => _observableTokenCount.Value;

    private readonly ObservableAsPropertyHelper<int> _observableTokenCount;

    // Collection to store token borders.
    public ObservableCollection<Border> TokensCollection { get; } = new ObservableCollection<Border>();

    // Properties for UI bindings.
    public bool IsUiVisible
    {
        get => _isUiVisible;
        set => this.RaiseAndSetIfChanged(ref _isUiVisible, value);
    }

    public bool IsAddVisible
    {
        get => _isAddVisible;
        set => this.RaiseAndSetIfChanged(ref _isAddVisible, value);
    }

    public double UiButtonOpacity
    {
        get => _uiButtonOpacity;
        set => this.RaiseAndSetIfChanged(ref _uiButtonOpacity, value);
    }

    // Commands for UI actions.
    public ReactiveCommand<Unit, Unit> ToggleUiVisibility { get; }
    public ReactiveCommand<Unit, Unit> ToggleAddVisibility { get; }



    public PlayerViewModel()
    {
        ToggleUiVisibility = ReactiveCommand.Create(ToggleUiToggleButton);
        ToggleAddVisibility = ReactiveCommand.Create(ToggleAddButton);


        _observableTokenCount = this
            .WhenAnyValue(vm => vm._tokenCount)
            .ToProperty(this, vm => vm.ObservableTokenCount);
    }

    // Toggles the visibility of the UI.
    private void ToggleUiToggleButton()
    {
        IsUiVisible ^= true;
        UiButtonOpacity = IsUiVisible ? 1.0 : 0.5;
    }

    // Toggles the visibility of the Add overlay.
    private void ToggleAddButton()
    {
        IsAddVisible ^= true;
    }


}