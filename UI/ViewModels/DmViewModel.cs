using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using ReactiveUI;
using UI.Classes;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using Point = Avalonia.Point;


namespace UI.ViewModels;

/// <summary>
/// ViewModel for the DmView.
/// </summary>
public class DmViewModel : ViewModelBase
{
    private float PanFactor => MapHandler.TileSize(TrueZoom);
    private const int MinZoom = 0;
    private const int MaxZoom = 6;

    // Private fields for UI state and token count.
    private bool _isUiVisible = true;
    private bool _isAddVisible;
    private double _uiButtonOpacity = 1.0;
    private int _tokenCount = 0;
    private Image _map;
    private float _x;
    private float _y;
    private Point? _prevPoint = null;
    private bool _panClicked = false;

    public bool PanClicked
    {
        get => _panClicked;
        set => this.RaiseAndSetIfChanged(ref (_panClicked), value);
    }

    public float X
    {
        get => _x;
        set => this.RaiseAndSetIfChanged(ref (_x), value);
    }

    public float Y
    {
        get => _y;
        set => this.RaiseAndSetIfChanged(ref (_y), value);
    }

    private float _zoom = 1.0f;
    public float TrueZoom => float.Pow(2, _zoom - 1);
    public string ZoomString => $"{TrueZoom:F1}x";
    public float Zoom
    {
        get => _zoom;
        set
        {
            if (value == _zoom) return;
            this.RaiseAndSetIfChanged(ref _zoom, value);
            OnZoom();
        }
    }

    public Image Map
    {
        get => _map;
        set => this.RaiseAndSetIfChanged(ref (_map), value);
    }
    private int _fogOfWarSize = 100;
    private bool _isFogOfWarVisible;
    private bool _isFogSliderVisible;
    public bool _isLeftMouseDown;

    private int ObservableTokenCount => _observableTokenCount.Value;

    private readonly ObservableAsPropertyHelper<int> _observableTokenCount;

    // Collection to store tokens
    public ObservableCollection<Token> TokensCollection { get; } = new ObservableCollection<Token>();

    public ObservableCollection<Token> TokensOnCanvas { get; } = new ObservableCollection<Token>();

    //Store Fog of War
    public ObservableCollection<FogOfWarShape> FogOfWarRectangles { get; } =
        new ObservableCollection<FogOfWarShape>();

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
    public ReactiveCommand<Unit, Unit> AddTokenCommand { get; }

    public DmViewModel()
    {
        ToggleUiVisibility = ReactiveCommand.Create(ToggleUiToggleButton);
        ToggleAddVisibility = ReactiveCommand.Create(ToggleAddButton);
        AddTokenCommand = ReactiveCommand.CreateFromTask(AddTokenAsync);
        LoadExistingImages();

        _observableTokenCount = this
            .WhenAnyValue(vm => vm._tokenCount)
            .ToProperty(this, vm => vm.ObservableTokenCount);
    }

    public int FogOfWarSize
    {
        get => _fogOfWarSize;
        set => this.RaiseAndSetIfChanged(ref _fogOfWarSize, value);
    }

    public bool IsFogOfWarVisible
    {
        get => _isFogOfWarVisible;
        set => this.RaiseAndSetIfChanged(ref _isFogOfWarVisible, value);
    }

    public bool IsFogSliderVisible
    {
        get => _isFogSliderVisible;
        set
        {
            this.RaiseAndSetIfChanged(ref _isFogSliderVisible, value);
            IsFogOfWarVisible = true;
        }
    }

    private void AddFogRectangleAt(Point position)
    {
        if (!_isLeftMouseDown) return;
        const double tolerance = 0.005;
        var offset = FogOfWarSize / 2.0;
        //TODO if shape is bigger place, if its smaller do not
        if (FogOfWarRectangles.Any(rect => Math.Abs(rect.XLoc - ((int)position.X - offset)) < tolerance &&
                                           Math.Abs(rect.YLoc - ((int)position.Y - offset)) < tolerance &&
                                           Math.Abs((int)(rect.Height - FogOfWarSize)) < tolerance &&
                                           Math.Abs(rect.Width - FogOfWarSize) < tolerance))
        {
            return;
        }

        (double x, double y) pos =
            MapHandler.ScreenToWorldspace(X, Y, Zoom, ((int)position.X - offset, (int)position.Y - offset));
        (double x, double y) rPos = MapHandler.WorldToScreenspace(X, Y, Zoom, (pos.x, pos.y));
        FogOfWarShape shape = new(pos.x, pos.y, rPos.x, rPos.y, FogOfWarSize);
        shape.updateSize(TrueZoom);
        Canvas.SetLeft(shape, shape.RelativeX);
        Canvas.SetTop(shape, shape.RelativeY);
        FogOfWarRectangles.Add(shape);
    }

    public void HandlePointerMoved(Point position, bool isRightPressed)
    {
        if (isRightPressed)
        {
            _isLeftMouseDown = false;
            HandleRightClick(position);
        }
        else
        {
            AddFogRectangleAt(position);
        }
    }

    public void HandlePointerFogOfWar(Point position, bool isRightPressed)
    {
        if (isRightPressed)
        {
            _isLeftMouseDown = false;
            HandleRightClick(position);
        }
        else
        {
            _isLeftMouseDown ^= true;
            AddFogRectangleAt(position);
        }
    }

    public void HandleRightClick(Point position)
    {
        var rectanglesToRemove = FogOfWarRectangles
            .Where(rect => IsPointInsideRectangle(position, rect))
            .ToList();

        foreach (var rect in rectanglesToRemove)
        {
            FogOfWarRectangles.Remove(rect);
        }
    }

    private bool IsPointInsideRectangle(Point point, FogOfWarShape rect)
    {
        return point.X >= rect.RelativeX && point.X <= rect.RelativeX + rect.Width &&
               point.Y >= rect.RelativeY && point.Y <= rect.RelativeY + rect.Height;
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

    // Adds a token to the TokensCollection.
    private async Task AddTokenAsync()
    {
        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            ?.MainWindow;
        Debug.Assert(mainWindow != null, nameof(mainWindow) + " != null");

        var storageProvider = mainWindow.StorageProvider;
        if (storageProvider.CanOpen)
        {
            var options = new FilePickerOpenOptions()
            {
                AllowMultiple = false
            };
            var files = await storageProvider.OpenFilePickerAsync(options);

            if (files is { Count: > 0 })
            {
                var validExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
                var selectedFile = files[0];
                var selectedFilePath = selectedFile.TryGetLocalPath() ?? selectedFile.Name;

                var fileExtension = Path.GetExtension(selectedFilePath).ToLowerInvariant();
                if (!validExtensions.Contains(fileExtension))
                {
                    Console.WriteLine($"File {selectedFilePath} has an invalid extension.");
                    return; // Exit the method early since the file is not valid.
                }

                var documentsDirectory = Directory.GetCurrentDirectory();
                const string appFolderName = ".worldcrucible";
                const string tokensFolderName = "Tokens";

                var appFolderPath = Path.Combine(documentsDirectory, appFolderName);
                var tokensFolderPath = Path.Combine(appFolderPath, tokensFolderName);

                if (!Directory.Exists(tokensFolderPath))
                {
                    try
                    {
                        Directory.CreateDirectory(tokensFolderPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating directory: {ex.Message}");
                    }
                }

                if (File.Exists(selectedFilePath) && Directory.Exists(tokensFolderPath))
                {
                    // Prompt the user to enter a new file name and await the result
                    var newName = await PromptForNewFileNameAsync(selectedFilePath);

                    if (string.IsNullOrWhiteSpace(newName))
                    {
                        Console.WriteLine("Invalid file name. Aborting.");
                        return;
                    }

                    var newFilePath = Path.Combine(tokensFolderPath, newName);

                    try
                    {
                        File.Copy(selectedFilePath, newFilePath, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error copying file: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Failed");
                }
            }
        }

        LoadExistingImages();
    }

    private async Task<string> PromptForNewFileNameAsync(string selectedFilePath)
    {
        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            ?.MainWindow;
        Debug.Assert(mainWindow != null, nameof(mainWindow) + " != null");
        // Create a new window for the prompt.
        var dialog = new Window
        {
            Width = 300,
            Height = 150,
            Title = "Enter New File Name",
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        // Create a TextBox for user input.
        var textBox = new TextBox
        {
            Watermark = "Enter new file name...",
            Text = Path.GetFileNameWithoutExtension(selectedFilePath),
            MaxLength = 9
        };

        // Create a Button for submission.
        var submitButton = new Button
        {
            Content = "Submit",
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 10, 0, 0)
        };

        // Create a TaskCompletionSource to await the user's input
        var tcs = new TaskCompletionSource<string>();

        // Handle the button click event.
        submitButton.Click += (sender, e) =>
        {
            // Set the result and complete the task
            tcs.SetResult($"{textBox.Text}{Path.GetExtension(selectedFilePath)}");
            dialog.Close();
        };

        // Arrange controls in a vertical stack.
        var stackPanel = new StackPanel
        {
            Margin = new Thickness(10),
            Children =
            {
                new TextBlock { Text = "Please enter a new file name:" },
                textBox,
                submitButton
            }
        };

        dialog.Content = stackPanel;
        await dialog.ShowDialog(mainWindow);

        // Await the user's input using the TaskCompletionSource
        return await tcs.Task;
    }


    // Loads existing images from the tokens folder and adds them to the TokensCollection.
    private void LoadExistingImages()
    {
        // Define the path to the tokens folder currently in documents, should probably change.
        var documentsDirectory = Directory.GetCurrentDirectory();
        const string appFolderName = ".worldcrucible";
        const string tokensFolderName = "Tokens";

        var tokensFolderPath = Path.Combine(documentsDirectory, appFolderName, tokensFolderName);

        // Check if the tokens folder exists.
        if (!Directory.Exists(tokensFolderPath)) return;

        // Retrieve all image files from the tokens folder.
        var validExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
        var imageFiles = Directory.GetFiles(tokensFolderPath, "*.*", SearchOption.TopDirectoryOnly)
            .Where(s => validExtensions.Contains(Path.GetExtension(s).ToLowerInvariant()));
        TokensCollection.Clear();

        foreach (var imageFilePath in imageFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(imageFilePath);
            // Create a bitmap from the image file.
            var bitmap = new Bitmap(imageFilePath);

            var token = new Token(fileName, new ImageBrush(bitmap));
            TokensCollection.Add(token);
        }
    }

    public void HandleTokenDrop(Token token, Point position, bool isDragging)
    {
        if (!string.IsNullOrEmpty(token.text.Text) && isDragging) return;

        // Calculate the half-width and half-height of the token based on its current size.
        double halfWidth = token.Size / 2.0; // Since the width is changed to 40 once added to canvas
        double halfHeight = token.Size / 2.0;

        (double x, double y) pos =
                MapHandler.ScreenToWorldspace(X, Y, TrueZoom, (position.X - halfWidth, position.Y - halfHeight));

        // Create a copy of the original token
        if (!token.OnCavas)
        {
            pos.x += 2 / TrueZoom - 1f / 2;
            pos.y += 2 / TrueZoom - 1f / 2;
            (double x, double y) Rpos = MapHandler.WorldToScreenspace(X, Y, TrueZoom, (pos.x, pos.y));
            var tokenCopy = new Token(token.Name, token.ImageBitMap)
            {
                XLoc = pos.x,
                YLoc = pos.y,
                RelativeX = Rpos.x - ((2 / TrueZoom) - token.Scaling / 2) * MapHandler.TileSize(TrueZoom),
                RelativeY = Rpos.y //+ (4/TrueZoom - 1)
            };
            // Add the token copy to the collection
            tokenCopy.Width = 40;
            tokenCopy.Children.Remove(token.text);
            TokensOnCanvas.Add(tokenCopy);
            Canvas.SetLeft(tokenCopy, tokenCopy.RelativeX);
            Canvas.SetTop(tokenCopy, tokenCopy.RelativeY);
            tokenCopy.OnCavas = true;
            tokenCopy.RequestDelete += OnTokenRequestDelete;
            tokenCopy.updateScaling(TrueZoom);
        }
        else
        {
            (double x, double y) Rpos = MapHandler.WorldToScreenspace(X, Y, TrueZoom, (pos.x, pos.y));
            token.XLoc = pos.x;
            token.YLoc = pos.y;
            token.RelativeX = Rpos.x - ((2 / TrueZoom) - token.Scaling / 2) * MapHandler.TileSize(TrueZoom);
            token.RelativeY = Rpos.y;
            Canvas.SetLeft(token, token.RelativeX);
            Canvas.SetTop(token, token.RelativeY);
            token.OnCavas = true;
            token.Pressed = false;
        }

        token.RequestDelete += OnTokenRequestDelete;
    }


    private void OnTokenRequestDelete(Token token)
    {
        token.ContextMenu.Close();
        TokensOnCanvas.Remove(token);
    }

    public bool IsTokenPressed()
    {
        foreach (Token token in TokensOnCanvas)
        {
            if (token.Pressed)
            {
                return true;
            }
        }

        return false;
    }

    public void UpdateTokens()
    {
        foreach (Token token in TokensOnCanvas)
        {
            (double x, double y) pos = MapHandler.WorldToScreenspace(X, Y, TrueZoom, (token.XLoc, token.YLoc));
            token.RelativeX = pos.x - ((2 / TrueZoom) - token.Scaling / 2) * MapHandler.TileSize(TrueZoom);
            token.RelativeY = pos.y;
            token.updateScaling(TrueZoom);
            Canvas.SetLeft(token, token.RelativeX);
            Canvas.SetTop(token, token.RelativeY);
        }
    }

    public void UpdateFOW()
    {
        foreach (FogOfWarShape shape in FogOfWarRectangles)
        {
            (double x, double y) pos = MapHandler.WorldToScreenspace(X, Y, TrueZoom, (shape.XLoc, shape.YLoc));
            shape.RelativeX = pos.x;
            shape.RelativeY = pos.y;
            shape.updateSize(TrueZoom);
            Canvas.SetLeft(shape, shape.RelativeX);
            Canvas.SetTop(shape, shape.RelativeY);
        }
    }
    public void Pan(Point point)
    {
        if (!_panClicked || IsTokenPressed())
        {
            if (IsTokenPressed())
            {
                EndPan();
            }
            return;
        }
        if (_prevPoint != null)
        {
            X = (float)(X - (point.X - _prevPoint.Value.X) / PanFactor);
            Y = (float)(Y - (point.Y - _prevPoint.Value.Y) / PanFactor);
            MapHandler.ClearBitmap();
            MapHandler.Render(X, Y, TrueZoom);
            MapHandler.RebindSource(Map);

            UpdateTokens();
            UpdateFOW();
        }

        _prevPoint = point;
    }

    public void EndPan()
    {
        _panClicked = false;
        _prevPoint = null;
    }

    public void OnScrollWheel(int scrollValue)
    {
        if (scrollValue > 0 && _zoom <= MinZoom || scrollValue < 0 && _zoom >= MaxZoom) return;
        Zoom -= scrollValue;
        OnZoom();
    }

    public void OnZoom()
    {
        WriteableBitmap buffer = MapHandler.Buffer;
        MapHandler.ClearBitmap();
        MapHandler.Render(X, Y, TrueZoom);
        MapHandler.RebindSource(Map);
        this.RaisePropertyChanged(nameof(ZoomString));
        UpdateTokens();
        UpdateFOW();
    }
}