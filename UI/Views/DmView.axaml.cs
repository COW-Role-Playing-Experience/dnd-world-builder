using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using UI.Classes;
using UI.ViewModels;

namespace UI.Views;

public partial class DmView : UserControl
{
    public DmView()
    {
        InitializeComponent();
        DataContext = new DmViewModel();
        var tokensItemsControl = this.FindControl<ItemsControl>("TokensOnCanvasControl");
        tokensItemsControl?.AddHandler(DragDrop.DropEvent, OnTokenDropped);
        tokensItemsControl?.AddHandler(DragDrop.DragOverEvent, OnTokenDragged);
        var map = this.FindControl<Image>("Map");
        MapHandler.RebindSource(map);
        (DataContext as DmViewModel).Map = map;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnTokenDragged(object sender, DragEventArgs e)
    {
        if (e.Data.Contains("Token") && e.Data.Get("Token") is Token token)
        {
            var position = e.GetPosition(this.FindControl<ItemsControl>("TokensOnCanvasControl"));

            // Delegate the logic to the ViewModel
            (DataContext as DmViewModel)?.HandleTokenDrop(token, position, true);
        }

    }

    private void OnTokenDropped(object sender, DragEventArgs e)
    {
        Console.WriteLine("TEST");
        if (e.Data.Contains("Token") && e.Data.Get("Token") is Token token)
        {
            var position = e.GetPosition(this.FindControl<ItemsControl>("TokensOnCanvasControl"));

            // Delegate the logic to the ViewModel
            (DataContext as DmViewModel)?.HandleTokenDrop(token, position, false);
        }
    }

    private void FogOfWarControl_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        var vm = DataContext as DmViewModel;
        var point = e.GetCurrentPoint(sender as Visual).Properties;

        if (vm?.IsFogOfWarVisible == true)
        {
            var position = e.GetPosition(this);
            if (point.IsRightButtonPressed)
            {
                vm.HandlePointerFogOfWar(position, true);
            }
            else if (point.IsLeftButtonPressed)
            {
                vm.HandlePointerFogOfWar(position, false);
            }
        }
        else
        {
            Panning_Pressed(sender, e);
        }
    }


    private void FogOfWarControl_OnPointerMoved(object sender, PointerEventArgs e)
    {
        var vm = DataContext as DmViewModel;
        var point = e.GetCurrentPoint(sender as Visual).Properties;

        if (vm?.IsFogOfWarVisible == true)
        {
            var position = e.GetPosition(this);
            if (point.IsRightButtonPressed)
            {
                vm.HandlePointerMoved(position, true);
            }
            else if (point.IsLeftButtonPressed)
            {
                vm.HandlePointerMoved(position, false);
            }
        }
        else
        {
            Panning_Moved(sender, e);
        }
    }


    private void FogOfWarControl_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        var vm = DataContext as DmViewModel;
        var point = e.GetCurrentPoint(sender as Visual).Properties;

        if (vm?.IsFogOfWarVisible == true)
        {
            var position = e.GetPosition(this);
            if (point.IsRightButtonPressed)
            {
                vm.HandlePointerFogOfWar(position, true);
            }
            else if (point.IsLeftButtonPressed)
            {
                vm.HandlePointerFogOfWar(position, false);
            }
        }
        else
        {
            Panning_Released(sender, e);
        }
    }

    private void Panning_Pressed(object? sender, PointerPressedEventArgs e)
    {
        (DataContext as DmViewModel).PanClicked = true;
    }

    private void Panning_Moved(object? sender, PointerEventArgs e)
    {
        (DataContext as DmViewModel).Pan(e.GetPosition(sender as Visual));
    }

    private void Panning_Released(object? sender, PointerReleasedEventArgs e)
    {
        (DataContext as DmViewModel).EndPan();
    }
}