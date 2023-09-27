using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
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
        var map = this.FindControl<Image>("Map");
        MapHandler.RebindSource(map);
        (DataContext as DmViewModel).Map = map;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }


    private void OnTokenDropped(object sender, DragEventArgs e)
    {
        if (e.Data.Contains("Token") && e.Data.Get("Token") is Token token)
        {
            var position = e.GetPosition(this.FindControl<ItemsControl>("TokensOnCanvasControl"));

            // Delegate the logic to the ViewModel
            (DataContext as DmViewModel)?.HandleTokenDrop(token, position);
        }
    }





}