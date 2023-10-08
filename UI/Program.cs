using Avalonia;
using Avalonia.ReactiveUI;
using map_generator.JsonLoading;
using System;
using UI.Classes;
using UI.ViewModels;
using UI.Views;

namespace UI;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        DataLoader.Init(); //load all the textures
        MapHandler.Pipeline = new(null, null); //create a new pipeline with unbound mapbuilder and WritableBuffer
        BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
