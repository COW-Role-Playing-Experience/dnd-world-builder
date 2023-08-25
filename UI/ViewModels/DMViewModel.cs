using System;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;

namespace UI.ViewModels;

public class DMViewModel : ViewModelBase
{
    private bool isUIVisible = true;
    private double uiButtonOpacity = 1.0;

    public bool IsUIVisible
    {
        get => isUIVisible;
        set => this.RaiseAndSetIfChanged(ref isUIVisible, value);
    }

    public double UIButtonOpacity
    {
        get => uiButtonOpacity;
        set => this.RaiseAndSetIfChanged(ref uiButtonOpacity, value);
    }

    public ReactiveCommand<Unit, Unit> ToggleUIVisibility { get; }

    public DMViewModel()
    {
        ToggleUIVisibility = ReactiveCommand.Create(ToggleButton);
    }

    private void ToggleButton()
    {
        IsUIVisible ^= true;

        UIButtonOpacity = IsUIVisible ? 1.0 : 0.5;
    }
}