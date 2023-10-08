using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Diagnostics;
using Avalonia.Markup.Xaml;
using UI.ViewModels;
using UI.Views;

namespace UI
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            // Initialize DevTools
            this.AttachDevTools();

            base.OnFrameworkInitializationCompleted();
        }
    }
}