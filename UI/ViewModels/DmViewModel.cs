using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using ReactiveUI;

namespace UI.ViewModels;

public class DmViewModel : ViewModelBase
{
    private bool isUIVisible = true;
    private bool isAddVisable;
    private double uiButtonOpacity = 1.0;
    private int circleCount;
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
        AddCircleCommand = ReactiveCommand.CreateFromTask(AddCircleAsync);

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

    private async Task AddCircleAsync()
    {
        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
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
                var selectedFile = files[0];
                var selectedFilePath = selectedFile.TryGetLocalPath() ?? selectedFile.Name;

                var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                const string appFolderName = ".worldcrucible";
                const string tokensFolderName = "Tokens";

                var appFolderPath = documentsDirectory + "/" + appFolderName;
                var tokensFolderPath = appFolderPath + "/" + tokensFolderName;

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
                    var selectedFileName = selectedFilePath[(selectedFilePath.LastIndexOfAny(new[] { '/', '\\' }) + 1)..];
                    var newFilePath = tokensFolderPath + "/" + selectedFileName;

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
    }

}