using Avalonia;
using Avalonia.Browser;
using AvaloniaHotMarkdown.Demo;
using ReactiveUI.Avalonia;
using System.Runtime.Versioning;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static Task Main(string[] args) => BuildAvaloniaApp()
            .WithInterFont()
#if DEBUG
            .WithDeveloperTools()
#endif
            .UseReactiveUI()
            .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}