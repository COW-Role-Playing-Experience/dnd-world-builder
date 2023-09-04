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
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ReactiveUI;
using Path = System.IO.Path;

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
                    var selectedFileName = Path.GetFileName(selectedFilePath);
                    var newFilePath = Path.Combine(tokensFolderPath, selectedFileName);

                    try
                    {
                        File.Copy(selectedFilePath, newFilePath, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error copying file: {ex.Message}");
                    }

                    var bitmap = new Bitmap(newFilePath);

                    var border = new Border
                    {
                        Width = 40,
                        Height = 40,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(20),
                        Background = new ImageBrush(bitmap),
                        ClipToBounds = true
                    };
                    TokensCollection.Add(border);
                }
                else
                {
                    Console.WriteLine("Failed");
                }
            }
        }
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

        foreach (var imageFilePath in imageFiles)
        {
            // Create a bitmap from the image file.
            var bitmap = new Bitmap(imageFilePath);

            // Create a bordered representation of the image.
            var border = new Border
            {
                Width = 40,
                Height = 40,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(20),
                Background = new ImageBrush(bitmap),
                ClipToBounds = true
            };

            TokensCollection.Add(border);
        }
    }
}