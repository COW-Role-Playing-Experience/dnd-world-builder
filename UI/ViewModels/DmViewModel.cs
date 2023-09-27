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

namespace UI.ViewModels;

/// <summary>
/// ViewModel for the DmView.
/// </summary>
public class DmViewModel : ViewModelBase
{
    // Private fields for UI state and token count.
    private bool _isUiVisible = true;
    private bool _isAddVisible;
    private double _uiButtonOpacity = 1.0;
    private int _tokenCount = 0;
    private Image _map;
    private int _zoom = 100;
    public int Zoom
    {
        get => _zoom;
        set => this.RaiseAndSetIfChanged(ref (_zoom), value);
    }

    public Image Map
    {
        get => _map;
        set => this.RaiseAndSetIfChanged(ref (_map), value);
    }

    private int ObservableTokenCount => _observableTokenCount.Value;

    private readonly ObservableAsPropertyHelper<int> _observableTokenCount;

    // Collection to store token borders.
    public ObservableCollection<Token> TokensCollection { get; } = new ObservableCollection<Token>();

    public ObservableCollection<Token> TokensOnCanvas { get; } = new ObservableCollection<Token>();

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

                var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
        var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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

    public void HandleTokenDrop(Token token, Point position)
    {
        // Calculate the half-width and half-height of the token based on its current size.
        double halfWidth = 40 / 2.0; // Since the width is changed to 40 once added to canvas
        double halfHeight = token.Size / 2.0;

        // Create a copy of the original token
        if (!token.OnCavas)
        {
            var tokenCopy = new Token(token.Name, token.ImageBitMap)
            {
                XLoc = (int)(position.X - halfWidth),
                YLoc = (int)(position.Y - halfHeight)
            };
            // Add the token copy to the collection
            tokenCopy.Width = 40;
            tokenCopy.Children.Remove(token.text);
            TokensOnCanvas.Add(tokenCopy);
            Canvas.SetLeft(tokenCopy, tokenCopy.XLoc);
            Canvas.SetTop(tokenCopy, tokenCopy.YLoc);
            Console.WriteLine($"Token added at X: {position.X}, Y: {position.Y}");
            tokenCopy.OnCavas = true;
            tokenCopy.RequestDelete += OnTokenRequestDelete;
        }
        else
        {
            token.XLoc = (int)(position.X - halfWidth);
            token.YLoc = (int)(position.Y - halfHeight);
            Canvas.SetLeft(token, token.XLoc);
            Canvas.SetTop(token, token.YLoc);
            Console.WriteLine($"Token moved at X: {position.X}, Y: {position.Y}");
            token.OnCavas = true;
        }

        token.RequestDelete += OnTokenRequestDelete;
    }


    private void OnTokenRequestDelete(Token token)
    {
        token.ContextMenu.Close();
        TokensOnCanvas.Remove(token);
    }

    public void Increase()
    {
        Zoom += 10;
        WriteableBitmap buffer = MapHandler.Buffer;
        MapHandler.ClearBitmap();
        MapHandler.Render(100f, 20f, (float)Zoom / 100);
        MapHandler.RebindSource(Map);
    }

    public void Decrease()
    {
        if (Zoom == 10)
        {
            return;
        }
        Zoom -= 10;
        WriteableBitmap buffer = MapHandler.Buffer;
        MapHandler.ClearBitmap();
        MapHandler.Render(100f, 20f, (float)Zoom / 100);
        MapHandler.RebindSource(Map);
    }

}