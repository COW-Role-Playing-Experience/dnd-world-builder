using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Brushes = Avalonia.Media.Brushes;

namespace UI.Classes
{
    public class Token : StackPanel
    {
        public bool OnCavas = false;
        public double XLoc { get; set; }
        public double YLoc { get; set; }
        public double RelativeX { get; set; }
        public double RelativeY { get; set; }
        public double Scaling { get; private set; }
        public double Zoom { get; set; }
        public double Size { get; set; }
        public bool Pressed { get; set; }
        public TextBlock text;
        public ImageBrush ImageBitMap { get; }
        private new string? Name { get; }
        public event Action<Token> RequestDelete;


        public new ContextMenu ContextMenu;

        public Token(string? fileName, ImageBrush imageBitMap)
        {
            ImageBitMap = imageBitMap;
            Scaling = 1;
            Size = 40;
            Name = fileName;
            Zoom = 1;

            text = new TextBlock
            {
                Text = fileName,
                TextAlignment = TextAlignment.Center,
                Width = 70
            };

            var border = new Border
            {
                Width = Size,
                Height = Size,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(Size / 2),
                Background = imageBitMap,
                ClipToBounds = true
            };

            var outerBorder = new Border
            {
                Background = Brushes.White,
                Width = Size,
                Height = Size,
                ClipToBounds = true,
                CornerRadius = new CornerRadius(Size / 2),
                Child = border,
            };


            Slider slider = new Slider
            {
                Minimum = 0.5,
                Maximum = 4,
                Value = 1,
                TickFrequency = 0.5,
                Margin = new Thickness(0, 5, 0, 5),
                IsSnapToTickEnabled = true
            };

            slider.ValueChanged += (s, e) =>
            {
                Scaling = e.NewValue;
                updateScaling(Zoom);
            };


            var deleteButton = new Button { Content = "Delete", Margin = new Thickness(0, 5, 0, 0) };
            deleteButton.Click += (s, e) => { RequestDelete?.Invoke(this); };

            ContextMenu = new ContextMenu();

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Children =
                {
                    new TextBlock { Text = "Adjust Scale", Margin = new Thickness(5) },
                    slider,
                    deleteButton
                }
            };
            Orientation = Orientation.Vertical;
            Children.Add(outerBorder);
            Children.Add(text);
            ContextMenu.Items.Add(stackPanel);
        }

        public void updateScaling(double zoom)
        {
            Size = MapHandler.TileSize((float)zoom) * Scaling/* * zoom*/;
            Zoom = zoom;
            Border outerBorder = (Border)Children[0];
            Border border = (Border)outerBorder.Child;
            border.Height = Size;
            border.Width = Size;
            border.CornerRadius = new CornerRadius(Size / 2);
            outerBorder.CornerRadius = new CornerRadius(Size / 2);
            outerBorder.Height = Size;
            outerBorder.Width = Size;
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
                Pressed = true;
                var data = new DataObject();
                data.Set("Token", this);
                DragDrop.DoDragDrop(e, data, DragDropEffects.Copy);
            }
            else if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed & OnCavas)
            {
                ContextMenu.Open(this);
            }
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            var position = e.GetPosition(this);
            Console.WriteLine($"Pointer released at X: {position.X}, Y: {position.Y}");
        }
    }
}