using UI.Classes;

namespace UI.ViewModels;

using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

public class PlayerViewModel : ViewModelBase
{
    // Private fields for UI state and token count.
    private bool _isUiVisible = true;

    private bool _isSeenWindowOpen = false;

    //Is seen tokens open
    private bool _isMonsterSeenVisible;
    private bool _isNpcSeenVisible;
    private bool _isPlayerSeenVisible;


    private double _uiButtonOpacity = 1.0;
    private int _tokenCount = 0;
    private int ObservableTokenCount => _observableTokenCount.Value;

    private readonly ObservableAsPropertyHelper<int> _observableTokenCount;

    // Collection to store token
    public ObservableCollection<Token> TokensVisible { get; } = new();

    // TODO Implement tokens being seperated by Monster/Player/Npc
    public ObservableCollection<Token> TokensMonster { get; } = new();
    public ObservableCollection<Token> TokensNpc { get; } = new();
    public ObservableCollection<Token> TokensPlayer { get; } = new();


    // Getters and settings for UI visibility
    public bool IsUiVisible
    {
        get => _isUiVisible;
        set => this.RaiseAndSetIfChanged(ref _isUiVisible, value);
    }

    public bool IsMonsterSeenVisible
    {
        get => _isMonsterSeenVisible;
        set
        {
            switch (_isSeenWindowOpen)
            {
                case false when _isMonsterSeenVisible == false:
                    _isSeenWindowOpen = true;
                    this.RaiseAndSetIfChanged(ref _isMonsterSeenVisible, value);
                    break;
                case true when _isMonsterSeenVisible:
                    _isSeenWindowOpen = false;
                    this.RaiseAndSetIfChanged(ref _isMonsterSeenVisible, value);
                    break;
            }
        }
    }

    public bool IsPlayerSeenVisible
    {
        get => _isPlayerSeenVisible;
        set
        {
            switch (_isSeenWindowOpen)
            {
                case false when _isPlayerSeenVisible == false:
                    _isSeenWindowOpen = true;
                    this.RaiseAndSetIfChanged(ref _isPlayerSeenVisible, value);
                    break;
                case true when _isPlayerSeenVisible:
                    _isSeenWindowOpen = false;
                    this.RaiseAndSetIfChanged(ref _isPlayerSeenVisible, value);
                    break;
            }
        }
    }

    public bool IsNpcSeenVisible
    {
        get => _isNpcSeenVisible;
        set
        {
            switch (_isSeenWindowOpen)
            {
                case false when _isNpcSeenVisible == false:
                    _isSeenWindowOpen = true;
                    this.RaiseAndSetIfChanged(ref _isNpcSeenVisible, value);
                    break;
                case true when _isNpcSeenVisible:
                    _isSeenWindowOpen = false;
                    this.RaiseAndSetIfChanged(ref _isNpcSeenVisible, value);
                    break;
            }
        }
    }

    public double UiButtonOpacity
    {
        get => _uiButtonOpacity;
        set => this.RaiseAndSetIfChanged(ref _uiButtonOpacity, value);
    }

    // Commands for UI actions.
    public ReactiveCommand<Unit, Unit> ToggleUiVisibility { get; }
    public ReactiveCommand<Unit, Unit> ToggleMonsterSeenVisibility { get; }
    public ReactiveCommand<Unit, Unit> ToggleNpcSeenVisibility { get; }
    public ReactiveCommand<Unit, Unit> TogglePlayerSeenVisibility { get; }


    public PlayerViewModel()
    {
        ToggleUiVisibility = ReactiveCommand.Create(ToggleUiToggleButton);
        ToggleMonsterSeenVisibility = ReactiveCommand.Create(ToggleMonsterSeenButton);
        ToggleNpcSeenVisibility = ReactiveCommand.Create(ToggleNpcSeenButton);
        TogglePlayerSeenVisibility = ReactiveCommand.Create(TogglePlayerSeenButton);

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

    // Toggles the visibility of the seen tokens
    private void ToggleMonsterSeenButton()
    {
        IsMonsterSeenVisible ^= true;
    }

    private void TogglePlayerSeenButton()
    {
        IsPlayerSeenVisible ^= true;
    }

    private void ToggleNpcSeenButton()
    {
        IsNpcSeenVisible ^= true;
    }
}