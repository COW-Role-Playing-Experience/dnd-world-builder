using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using ReactiveUI;

namespace UI.ViewModels;

public class DmViewModel : ViewModelBase
{
    private bool isUIVisible = true;
    private bool isAddVisable = false;
    private double uiButtonOpacity = 1.0;
    private int circleCount = 0;
    private int ObservableCircleCount => observableCircleCount.Value;

    private readonly ObservableAsPropertyHelper<int> observableCircleCount;
    public ObservableCollection<Ellipse> CirclesCollection { get; } = new ObservableCollection<Ellipse>();


    public bool IsUiVisible
    {
        get => isUIVisible;
        set => this.RaiseAndSetIfChanged(ref isUIVisible, value);
    }

    public bool IsAddVisible
    {
        get => isAddVisable;
        set => this.RaiseAndSetIfChanged(ref isAddVisable, value);
    }

    public double UiButtonOpacity
    {
        get => uiButtonOpacity;
        set => this.RaiseAndSetIfChanged(ref uiButtonOpacity, value);
    }

    public ReactiveCommand<Unit, Unit> ToggleUiVisibility { get; }
    public ReactiveCommand<Unit, Unit> ToggleAddVisibility { get; }

    public ReactiveCommand<Unit, Unit> AddCircleCommand { get; }

    public DmViewModel()
    {
        ToggleUiVisibility = ReactiveCommand.Create(ToggleUiToggleButton);
        ToggleAddVisibility = ReactiveCommand.Create(ToggleAddButton);
        AddCircleCommand = ReactiveCommand.Create(AddCircle);

        observableCircleCount = this
            .WhenAnyValue(vm => vm.circleCount)
            .ToProperty(this, vm => vm.ObservableCircleCount);
    }


    private void ToggleUiToggleButton()
    {
        IsUiVisible ^= true;
        UiButtonOpacity = IsUiVisible ? 1.0 : 0.5;
    }

    private void ToggleAddButton()
    {
        IsAddVisible ^= true;
    }



    private void AddCircle()
    {
        circleCount++;

        var circle = new Ellipse
        {
            Width = 40,
            Height = 40,
            Fill = Brushes.LightGray,
            Stroke = Brushes.Black,
            StrokeThickness = 1,
            Margin = new Thickness(5.0),
        };

        CirclesCollection.Add(circle);
    }
}