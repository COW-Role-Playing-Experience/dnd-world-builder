using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using SkiaSharp;
using UI.Views;
using Brushes = Avalonia.Media.Brushes;

namespace UI.Classes;

public class Token : StackPanel
{

    public bool OnCavas = false;
    public int XLoc { get; set; }
    public int YLoc { get; set; }

    public ImageBrush ImageBitMap { get; }
    private string? Name { get; }


    public Token(string? fileName, ImageBrush imageBitMap)
    {
        ImageBitMap = imageBitMap;

        Name = fileName;
        var text = new TextBlock
        {
            Text = fileName,
            TextAlignment = TextAlignment.Center
        };

        var border = new Border
        {
            Width = 40,
            Height = 40,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(20),
            Background = imageBitMap,
            ClipToBounds = true
        };

        Width = 70;
        Orientation = Orientation.Vertical;
        Children.Add(border);
        Children.Add(text);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        PointerPressed += OnPointerPressed;
        PointerReleased += OnPointerReleased;
    }


    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        PointerPressed -= OnPointerPressed;
        base.OnDetachedFromVisualTree(e);
        PointerReleased -= OnPointerReleased;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Console.WriteLine(Name);

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var data = new DataObject();
            data.Set("Token", this);
            DragDrop.DoDragDrop(e, data, DragDropEffects.Copy);
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var position = e.GetPosition(this);
        Console.WriteLine($"Pointer released at X: {position.X}, Y: {position.Y}");
    }
}